***api-scraper***

### Prerequisites

Run in elevated command prompt `netsh http add urlacl url="http://+:8888/haxton/" user="Everyone"`

.NET 4.6.1 
nuget package manager 

> note: there is no reliance on IIS or IIS Express. Everything is self hosted and can be run without administrator given the command above

### API Calls
* [POST]	`/scrape/`
You can post a list of [URLs, CacheLengthLimitSeconds] in a batch setting. See example:
```
[
    {"URLs":["http://google.com", "http://google.com/1"],"CacheLengthSeconds":3600.0},
    {"URLs":["http://google.com/2"],"CacheLengthSeconds":3604.0}
]
```

 This will produce an output similar to 
```
		[
			{
				"Url": "http://google.com",
				"DateReceived": "2018-02-15T08:42:00.2242978Z",
				"DateCompleted": "0001-01-01T00:00:00",
				"CacheExpiresOn": "2018-02-15T10:42:00.2242978Z",
				"CacheLength": "02:00:00",
				"Status": "completed",
				"Id": "42b4b082-99f3-4aca-a0bc-a5d29baf6fa5"
			},
			{
				"Url": "http://google.com/1",
				"DateReceived": "2018-02-15T08:42:00.2283009Z",
				"DateCompleted": "0001-01-01T00:00:00",
				"CacheExpiresOn": "2018-02-15T10:42:00.2283009Z",
				"CacheLength": "02:00:00",
				"Status": "accepted",
				"Id": "c63c862a-f72a-4ea4-ae21-b7d3c15031c9"
			},
			{
				"Url": "http://google.com/2",
				"DateReceived": "2018-02-15T08:42:00.2318047Z",
				"DateCompleted": "0001-01-01T00:00:00",
				"CacheExpiresOn": "2018-02-15T10:42:00.2318047Z",
				"CacheLength": "02:00:00",
				"Status": "accepted",
				"Id": "8a43c5bf-7d8e-4a4d-bccf-b2f90ee5a0e1"
			}
		]
```
* [GET]	`/scrape/{guid}`
		This endpoint will return the raw HTML of the id provided. You receive this guid (synonymous in this case with id) from the request you make. 
* [GET]	`/scrape/{guid}/status`
		Returns the status of the job. Essentially the same reponse you were provided when originally POSTing, but with updated data.
* [GET]	`/jobs/`
		For debugging purposes a list of all jobs that have been accepted into the queue but not scraping or finished. 
### Work that could be improved
* Well theres a bunch of things that could be done to improve this. One option would be to have the user provide the guids for us so they don't have to rely on us creating and returning them. Not much in this instance would chance the actual code, but it'd be a nice QoL change for less state to keep track of.
* You could easily create a `/scrap/{guid}/live` endpoint that has live updates as the scrape comes in, would benefit for not having to make significant API calls to see if the status has changed on my job or not.
* Being able to weigh jobs, such that one may be worth more than another. 
* There may be an extremely small race condition if you were able to start or fail a job with the same URL in a short period of time it may ignore the cache and re-attempt to scrape that page. Although the point is rather moot considering we have no actual cache limit. 
* The default cache time currently is 2 hours. While nice and all, it should be more explict than that, because if you set `CacheLengthSeconds` to `0` it will re-set itself to 2 hours. Not ideal. 
* If you wish to solve this without a database all you have to do is write a memory store version and implement IDataStore. Granted there currently exists a bit of state change / business logic within `DataStore` which _should_ be refactored out. 
* Currently using 3 workers. Should provide a config file to up or down it. Same with how often they run (currently 10 seconds)
* We are also kind of cheating with structuremap by using `WithDefaultConventions`. Should make everything very explicit and less magicy. 
* There is no way currently to DELETE a job or stop one from running. Fairly simple implementation, just pass around some CancellationTokens and call the source.Cancel when required.
* I'm sure there's other things I'm forgetting, but it's pretty solid and stands on its own feet perfectly fine. 