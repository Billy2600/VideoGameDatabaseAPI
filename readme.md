#Video Game Database API

This project is a RESTful API, written in C# .NET MVC, which provides information on video games, game consoles, and more.

##Paths
Here is a list of the most common paths. All paths begin with `api/`.

###`GET` api/Game
Retrieves list of games. Example:
```
{
    "gameId": 3,
    "gameName": "Sonic the Hedgehog",
    "releaseDate": "1991-06-23T00:00:00",
    "publisherId": 9,
    "consoleId": 15,
    "publisherName": "Sega Enterprises Ltd.",
    "consoleName": "Sega Genesis",
    "genres": [
        "Action",
        "Platformer"
    ]
},
{
    "gameId": 4,
    "gameName": "Sonic the Hedgehog 2",
    "releaseDate": "1992-11-21T00:00:00",
    "publisherId": null,
    "consoleId": 15,
    "publisherName": "Sega Enterprises Ltd.",
    "consoleName": "Sega Genesis",
    "genres": [
        "Action",
        "Platformer"
    ]
}
```

##`GET` api/Game/{id}
Retrieves a single game with the specified ID. Example, with the ID `3` specified:
```
{
    "gameId": 3,
    "gameName": "Sonic the Hedgehog",
    "releaseDate": "1991-06-23T00:00:00",
    "publisherId": 9,
    "consoleId": 15,
    "publisherName": "Sega Enterprises Ltd.",
    "consoleName": "Sega Genesis",
    "genres": [
        "Action",
        "Platformer"
    ]
}
```

##`PUT` api/Game/{id}
Updates a game with the specified ID with the JSON contained in the body of the request. The ID in the path and the object _must_ match. Will return nothing on successful update. Example body:
```
{
    "gameId": 3,
    "gameName": "Sonic the Hedgehog. With Update"
}
```

##`POST` api/Game
Inserts a new game. Game ID and Publisher ID need not be specified. Publisher ID will automatically be mapped based on PublisherName, and if publisher doesn't exist in the database, it will be added. Example:
```
{
    "gameName": "Sonic the Hedgehog 3",
    "releaseDate": "1994-02-02T00:00:00",
    "consoleId": 15,
    "publisherName": "Sega Enterprises Ltd.",
}
```