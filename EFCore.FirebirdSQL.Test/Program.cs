﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq; 

namespace EFCore.FirebirdSqlSQL.Test
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("# Wait... ");
            var cx = new Context();
            Console.WriteLine("# Deleting database...\n");
            cx.Database.EnsureDeleted();
            cx.Database.EnsureCreated();

            for (int i = 0; i < 100; i++)
            {
                cx.TestGuid.Add(new TestGuid
                {
                    FirstName = "Ralms"
                });
            } 

            //cx.Author.Add(new Author
            //{
            //    FirstName = "Rafael",
            //    LastName = "Almeida",
            //    Date = DateTime.Now.AddMilliseconds(1),
            //     Books = new List<Book>
            //                 {
            //                      new Book {  Title="Firebird 3.0.1"},
            //                      new Book {  Title="Firebird 3.0.2"}
            //                 } 
            //});
             
            cx.SaveChanges(); 

            var Guids = cx.TestGuid.ToList(); 
            foreach (var item in Guids)
            {
                Console.WriteLine($"  #->{item.Id} {item.FirstName}"); 
            }

            //Use Include (UPPER)
            //var Authors = cx.Author.Include(p => p.Books).Where(p => p.LastName.ToUpper().Contains("L")).ToList();
            //var Authors = cx.Author.Include(p => p.Books).Where(p => p.Date >= DateTime.Parse("2017-01-01")  && p.Date <= DateTime.Now).ToList();
             
            //Console.WriteLine($"-----------------------------------");
            //Console.WriteLine("             Author                 ");
            //Console.WriteLine($"-----------------------------------");
            //foreach (var item in Authors)
            //{
            //    Console.WriteLine($"  #->{item.FirstName} {item.LastName}");
            //    Console.WriteLine($"--------------BOOKS----------------");
            //    foreach (var book in item.Books) 
            //        Console.WriteLine($"{book.Title}"); 
            //} 
            Console.ReadKey();
        }
    }
}
