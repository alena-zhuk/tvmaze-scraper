# TV Maze Scraper
Simple scraper of the TV Maze API to retrieve and store shows and cast data for future use. 
1. scrapes the TVMaze API for show and cast information;
2. persists the data in storage;
3. provides the scraped data using a REST API

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
2.  

## Restrictions
1. TV Maze API calls are rate limited to allow at least 20 calls every 10 seconds per IP address.
2. 
