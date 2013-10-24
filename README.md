RabbitDB
========

A simple abstraction of data access. Simple, feature-rich, clean and hopefully efficent :)

Simple
----------

No dynamics, simple and no obscure command execution. 
If used by convention - no attributes are needed.

Features
------------

Change tracking, identity map, multiple resultsets, custom mappings
(all on request - soon to come).
Seperate `Entity` class to inherit from if you want to work with it without using within `DbSession`.

```csharp
[Table("Posts")]
class Post : Entity
{
    public int Id { get; set; }
    public int AuthorId { get; set; }
    public string Title { get; set; }
    public DateTime CreatedOn { get; set; }
    public PostType Type { get; set; }
    public int? TopicId { get; set; }
    public bool IsActive { get; set; }
}
```
Let´s assume you want to use an other primary key then `ID`:
```charp
[Table("Posts", AlternativePKs="FirstID, SecondID")]
class Post
{
    public int Id { get; set; }
    public ....
}
```

If you name your class like your table and your properties like your columns you don´t need attributes.
All needed information like dbtype, primarykey(s), default value, nullable is gathered by rabbitDB for you.

If you decide to inherit from `Entity` you have to register your connection string and the used DbEngine.
```csharp
Registrar<string>("Company.Module.*", @"YourConnectionString");
Registrar<DbEngine>("Company.Module.*", DbEngine.SqlServer);
```

Clean
-----

Based on clean-code principles and SOLID principles.

Efficient
---------

I don´t know yet.
