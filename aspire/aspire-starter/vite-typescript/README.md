# Description

This project serves as a frontend consumer of [AspNetApi](../AspNetApi/README.md)

It's a Vite app (Nodejs) with a button displaying a weather forecast table.

Install Node and Aspire CLI, and run with the rest of the resources with `aspire run`

It defines a proxy in `vite.config.ts` where any `/api` paths will be routed to the API. This is to overcome the CORS issue where it fails to fetch requests from another URL.
