# embark
Easy to use document database with only a few commands,
so that you don't have to learn a whole new framework to get going.. ideal for projects with an agile code-first approach, or if you just want something for now while prototyping.

##Visibility

Documents are saved in .txt files in folders for each collection so you can easily view/edit data while developing. The default serialization is Json, but you can also plug in some other format like YAML or DSON if you like.

##Simplicity

```csharp
// arrange some guinea pig
var pet = new Sheep { Name = "Fluffy", FavouriteIceCream = IceCream.Vanilla };

// save data locally
var db = Embark.Client.GetLocalDB(@"C:\AnimalsDB\"); /* Client.GetLocalDB() defaults to: Directory.GetCurrentDirectory() */

// or over a network (via REST API)
var io = Embark.Client.GetNetworkDB("127.0.0.1", 8080);// Not implemented, yet..

// collections created on-the-fly if needed
var io = db["sheep"];

// insert
long id = io.Insert(pet);

// get
Sheep fluffy = io.Select<Sheep>(id);

// update
fluffy.FavouriteIceCream = IceCream.Strawberry;
bool fluffyNowLikesStrawberry = io.Update(id, fluffy);

// delete
bool hasSheepVanished = io.Delete(id);
```
###All the commands are

####basic:
- Insert(object)
- Update(id, object)
- Get(id)
- Delete(id)

####range:
- GetLike(new { Name = "Rocket"})
- UpdateLike(new document, ..) 
- DeleteLike(..)
- GetBetween(new { Score = 15}, new { Score = 39.21})
- UpdateBetween(..)
- DeleteBetween(..)

####other:
- byte[] GetByteArray(object blob) to help with blob deserialization

**That's it!**

The intent of embark is to stay clean, simple and minimal..
Extra features like database replication, backup, user rights, etc.. will be done in another project ([splash](https://trello.com/splashdb), coming soon!) so that the core embark itself will remain crisp and friendly :)

##Usage

You can add the embark client [Nuget package](http://example.todo/), then copy paste the sample code and then simply continue developing right away.

If you want to save data over a network then download and run the server from [here](http://example.todo/). If you prefer to know the running code, feel free to download and compile - it will only take a minute.

To check out current developments go to [embarkdb on trello](https://trello.com/embarkdb)
