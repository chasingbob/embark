# embark
Easy to use document database with only a few commands, so that you don't have to learn a whole new framework to get going. Embark allows you to defer the plumbing or commitment of choosing a specific database technology before getting things done. Ideal for prototyping or projects with an agile code-first approach.

##Visibility

Documents are saved in .txt files in folders for each collection so you can easily view/edit data while developing. The default serialization is Json, but you can also plug in some other format like YAML or DSON if you like.

##Simplicity

```csharp
// arrange some guinea pig
var pet = new Sheep { Name = "Fluffy", FavouriteIceCream = IceCream.Vanilla };

// save data locally
var db = Embark.Client.GetLocalDB(@"C:\AnimalsDB\"); /* Client.GetLocalDB() defaults to: Directory.GetCurrentDirectory() */

// or over a network via REST API to WCF server *see usage section below*
var io = Embark.Client.GetNetworkDB("192.168.1.24", 8080);

// collections created on-the-fly if needed
var io = db.GetCollection<Sheep>("sheep");

// insert
long id = io.Insert(pet);

// get
Sheep fluffy = io.Select(id);

// update
fluffy.FavouriteIceCream = IceCream.Strawberry;
bool fluffyNowLikesStrawberry = io.Update(id, fluffy);

// delete
bool hasSheepVanished = io.Delete(id);

// non-type specific collection if you want to save Apples & Oranges in the same fruit collection
var io = db["fruit"];
```
###All the commands are

####basic:
- Insert(object) returns Int64 ID
- Select(id) returns document
- Update(id, object) returns bool successful
- Delete(id) returns bool successful

####range:
- SelectLike(new { Name = "Rocket"})
- SelectBetween(new { Score = 15}, new { Score = 39.21})
- SelectAll() returns Document Wrapper with ID & timestamp

####other:
- byte[] GetByteArray(object blob) to help with blob deserialization

####[in development](https://trello.com/b/rtqlPmrM/development):
- Aggregate functions (Count/Average/Min/Max..)
- More/better feedback from server
- Review & Simplify code

**That's it!**

The intent of embark is to stay clean, simple and minimal.. with a complete source code download of under 50 kilobytes, and no external dependencies other than the .NET framework.

Extra features like database replication, backup, user rights, security, etc.. and to be a general end-all solution, is not the aim of this project. By dodging increasing complexity from scope-creep, the core embark itself will remain crisp and friendly :)

##Usage

You can add the [Embark NuGet package](https://www.nuget.org/packages/Embark/), copy paste the sample code and then simply continue developing right away.

If you want to save data over a network: 
```csharp
// start a new server
var server = new Embark.Server();
server.Start();
```
or you can download and a simple server from [here](https://trello-attachments.s3.amazonaws.com/54f89f538ec1e186a911c534/5527fc8a8a55d94cbed0ab17/c3e0c011826d1fe4519a46f07e46b97e/BasicServer.zip). 
NOTE either run the server in admin mode or [allow your server app to use the your-machine:port/embark/ uri ](http://stackoverflow.com/a/17242260/4650900)

##Development

To check out current developments go to [embarkdb on trello](https://trello.com/b/rtqlPmrM/development) & please feel free to contact me @ embarkDB@gmail.com with any feedback, suggestions or to get involved!
