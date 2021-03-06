/*                 
 *     EntityFrameworkCore.FirebirdSqlSQL  - Congratulations EFCore Team
 *              https://www.FirebirdSqlsql.org/en/net-provider/ 
 *     Permission to use, copy, modify, and distribute this software and its
 *     documentation for any purpose, without fee, and without a written
 *     agreement is hereby granted, provided that the above copyright notice
 *     and this paragraph and the following two paragraphs appear in all copies. 
 * 
 *     The contents of this file are subject to the Initial
 *     Developer's Public License Version 1.0 (the "License");
 *     you may not use this file except in compliance with the
 *     License. You may obtain a copy of the License at
 *     http://www.FirebirdSqlsql.org/index.php?op=doc&id=idpl
 *
 *     Software distributed under the License is distributed on
 *     an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either
 *     express or implied.  See the License for the specific
 *     language governing rights and limitations under the License.
 *
 *              Copyright (c) 2017 Rafael Almeida
 *         Made In Sergipe-Brasil - ralms@ralms.net 
 *                  All Rights Reserved.
 */


using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query.Expressions;

namespace Microsoft.EntityFrameworkCore.Query.ExpressionTranslators.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class FirebirdSqlStartsWithOptimizedTranslator : IMethodCallTranslator
    {
        private static readonly MethodInfo _methodStringOf
            = typeof(string).GetRuntimeMethod(nameof(string.StartsWith), new[] { typeof(string) });

        private static readonly MethodInfo _methodCharOf
            = typeof(string).GetRuntimeMethod(nameof(string.StartsWith), new[] { typeof(char) });

        static readonly MethodInfo _concatCast
           = typeof(string).GetRuntimeMethod(nameof(string.Concat), new[] { typeof(string), typeof(string) });


        public virtual Expression Translate(MethodCallExpression methodStartCall)
        {
            if (!methodStartCall.Method.Equals(_methodStringOf) ||
                !methodStartCall.Method.Equals(_methodCharOf) ||
                methodStartCall.Object == null)
                return null;

            var constantPatternExpr = methodStartCall.Arguments[0] as ConstantExpression; 
            if (methodStartCall != null)
            {
                // Operation Simple With LIKE Sample (LIKE 'FIREBIRD%')
                return new LikeExpression(
                    methodStartCall.Object,
                    Expression.Constant(System.Text.RegularExpressions.Regex.Replace((string)constantPatternExpr.Value, @"([%_\\'])", @"\$1") + '%')
                );
            } 

            var pattern = methodStartCall.Arguments[0];
            return Expression.AndAlso(
                new LikeExpression(methodStartCall.Object, Expression.Add(pattern, Expression.Constant("%"), _concatCast)),
                Expression.Equal(
                    new SqlFunctionExpression("LEFT", typeof(string), new[]
                    {
                        methodStartCall.Object,
                        new SqlFunctionExpression("CHARACTER_LENGTH", typeof(int), new[] { pattern }),
                    }),
                    pattern
                )
            );
              
        }
    }
}
