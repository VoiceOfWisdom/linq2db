﻿module Tests.FSharp.WhereTest

open Tests.FSharp.Models

open LinqToDB
open LinqToDB.Mapping
open NUnit.Framework

let private TestOnePerson id firstName middleName lastName gender persons = 
    let list = persons :> Person System.Linq.IQueryable |> Seq.toList
    Assert.AreEqual(1, list |> List.length )

    let person = list |> List.head

    Assert.AreEqual(person, {
        Person.ID = id
        FirstName = firstName
        MiddleName = middleName
        LastName = lastName
        Gender = gender })

let TestOneJohn = TestOnePerson 1 "John" None "Pupkin" Gender.Male

let TestMethod() = 
    1

let LoadSingle (db : IDataContext) = 
    let persons = db.GetTable<Person>()
    TestOneJohn(query {
        for p in persons do
        where (p.ID = TestMethod())
        select p
    })



let LoadSingleComplexPerson (db : IDataContext) = 
    let persons = db.GetTable<ComplexPerson>()
    let john = query {
        for p in persons do
        where (p.ID = TestMethod())
        exactlyOne
    }
    Assert.AreEqual(
        { ComplexPerson.ID=1
          Name = {FirstName="John"; MiddleName=null; LastName="Pupkin"}
          Gender="M" }
        , john)

let LoadSingleDeeplyComplexPerson (db : IDataContext) = 
    let persons = db.GetTable<DeeplyComplexPerson>()
    let john = query {
        for p in persons do
        where (p.ID = TestMethod())
        exactlyOne
    }
    Assert.AreEqual(
        { DeeplyComplexPerson.ID=1
          Name = {FirstName="John"; MiddleName=null; LastName={Value="Pupkin"}}
          Gender="M" }
        , john)

let LoadColumnOfDeeplyComplexPerson (db : IDataContext) = 
    let persons = db.GetTable<DeeplyComplexPerson>()
    let lastName = query {
        for p in persons do
        where (p.ID = TestMethod())
        select p.Name.LastName.Value
        exactlyOne
    }
    Assert.AreEqual("Pupkin", lastName)
