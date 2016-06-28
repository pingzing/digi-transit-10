#DigiTransit 10 (name not final)
It's an HSL DigiTransit app for Helsinki. Written as a UWA.

##Building
Requirements
* [Multilingual App Toolkit v4.0](https://developer.microsoft.com/en-us/windows/develop/multilingual-app-toolkit)
* [Resw Code File Generator](https://reswcodegen.codeplex.com/)


###Design Notes
####Searching
The official DigiTransit webapp does two things when the user enters a search term: Makes a GraphQL call for stops based on the search term, and a call to the REST endpoint for geocoding with the search string. It then collects the results, sorts them, and distinguishes between them with different icons in the autosuggest list. We should copy that strategy.

####Poorly-documented GraphQL Info
Time and date parameters, when requested as strings, are expected to look like this:
```
date: "2016-05-20",
time: "23:28:00"
```

I.E. dates are in ISO 8601 format, and times are expressed in 24-hour clock format with colon separators. 

When sending JSON requests, the double-quotes around strings must be escaped _in the request_. That means backslashes next to the quotes. _In the request_. Ugh.

When times are returned by the server, they are usually in UNIX timestamp format, encoded as `long`s.