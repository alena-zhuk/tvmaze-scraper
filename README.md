# TV Maze Scraper
Simple scraper of the TV Maze API to retrieve and store shows and cast data for future use. It consist of two applications:
1. WorkerService - a background process which checks updates daily and downloads updated shows into the storage. On the first run it will download all entities.
2. API - provides a set of endpoints to access the data in the storage
Run both applications simultaniously. First scrape might take hours if you are unlucky with the cache on TVMaze side. However all downloaded data is available via API on the fly.


## Requirements
### Business
1. Scraped data is exposed via REST API
2. The api endpoint provides a paginated list of all tv shows containing the id of the TV show and a list of
all the cast that are playing in that TV show.
3. The list of the cast must be ordered by birthday descending.
### Technical
1. Scraped data is persisted in a storage
2. The API endpoint returns result in JSON format
   
## Assumptions
1. The Scraper API is public and requires no authentication nor authorization
2. Actual information is crucial for the business, therefor Scraper has to run continiously and check for updates

## Restrictions
1. TV Maze API calls are rate limited to allow at least 20 calls every 10 seconds per IP address.
2. It's not possible to list all shows from TVMaze and embed cast in the response. Separate requests should be performed for each show.

Time estimate for a full scrape: 
 ~60 000 shows 
 20 req per 10 sec = 120 req per 1 min 
 60 000 / 120 = 500 min = > 8h (if nothing is cached)
