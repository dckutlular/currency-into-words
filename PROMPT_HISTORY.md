# Prompt History

````text
I will create web app with .net 10 and blazor. Show me the best practices for scaffolding a new project.
How would you break down into projects like web, api, business logic, test. Follow a clean architecture and best practices.
We will build a conversion app which will take user input as number and we will convert it into words.
- For each project, say what it references and why.
- dont over engineer.
- Explain the structure first; don't create any files yet.
````

````text
Before we write any code: how should the Core be designed? Which classes do we need, and what should the first test be?
lets start with core project.
````
````text
look good but we can't parse directly cents into integers: the part after the comma, parsed as an integer (0–99).
25,1 means 10 cents not 1.
we should have max 2 digits for cents. 
````

````text
Agree.
````

````text
Write AmountParser, test-first. 
Write the tests from our agreed table (digits after comma), show me that they fail, then write the code.

make sure you stick these points: (Rules) 
- spaces as thousands separators
- one decimal digit means tenths, the agreed fix
- more than 2 decimals is invalid
- comma with nothing after it is invalid
- more than one comma is invalid
- make sure it is a number not a letter
- negative amounts aren't supported
- max number is 999999999.
````

````text
Return a CurrencyAmount with dollars and cents.
````

````text
I want 2 exception types: FormatException for input that is not a valid amount and ArgumentOutOfRangeException for out of range values.
each exception message says whats wrong in words that a user can understand
````

````text
program error and message shown to user should be different. user shouldnt see the real error message.
I want to support 2 languages for error messages based on users selection. parser takes language code as param.
No error-code enums or custom exception classes. no overkill.
````

````text
move these constant messages into resource files for english and german (.resx)
````

````text
Next is the English converter, test-first. it implements ICurrencyToWordsConverter and takes a CurrencyAmount.
Start with 0 dollars and add one rule per test, like in our plan. Show me each red/green step.
````

````text
logic looks correct, check some extra cases plus these following:
1) 25,1 means ten cents, not one. The cents part is padded on the right.
2) 0 alone is "zero dollars" with no cents part; 0,01 still says "zero dollars".
3) numbers with space, numbers with more than 1 space
````


````text
next is the German converter, test-first again. with the same interface. before you write any tests, list the German rules that are different from English, so we can agree on the expected outputs first.
Where German has more than one accepted form ("hundert" vs. "einhundert"), show the options and recommend one. 
- don't write any tests until I confirm the table
````

````text
Agreed on A 100(einhundert) --not hundert and B 101(einhunderteins Dollar), and the table is correct. 
Write the German tests from it, show me red, then make them green step by step.
101_000_000 should return einhunderteins Millionen Dollar.
````

````text
The Core is finished. next is the API. Which project template should I use, what should the endpoint look like, and how should the request and response be shaped? Explain first, then we build it.
````

````text
implement with these points:
- Project template, and minimal API or controllers for a single endpoint.
- The endpoint method, route, and how it picks the English or German converter.
- error responses for bad input, an unsupported language and unexpected errors: status codes and body shape.
- The Blazor client will call this endpoint. should the request and response types be shared, and how?
````

````text
Add the request and response records, register both converters, and write POST /api/convert. Show it working with a request in the .http file.
````

````text
lets implement error handling. 
Bad amounts and unknown languages should return 400 ProblemDetails with the right message, 
and unexpected errors a generic 500. Show me all three cases.
add a request for each 400 to the .http file, and show the 500 with a temporary throw that you remove afterwards
````

````text
api part should be sufficient now. lets continue with UI. I want to use Blazor web assembly. 
show me the plan first and we will build together once we agree.
- standalone WebAssembly or a blazor web app with webAssembly interactivity? Recommend one.
````


````text
how does the UI get the request and response records? the web project must not reference the core api project.
conversion must only run on the server, so the browser must never download Core
````

````text
create a small CurrencyToWords.Contracts project referenced by both Api and Web
````

````text
create the empty Blazor WebAssembly project, delete the template pages, and show me it starts.
````


````text
add a convert page, happy path only. An amount field, a language selector, a Convert button, and the result. Nothing more.
````


````text
 Recommend a few improvements that would make the convert page look more modern and easier to use. Use design.png as the reference. Consider at least:
 - fields next to each other
 - flags in the language selector
 - the result in a card with a copy button
 - a clear place for errors

 List them first, ordered by impact versus effort, with what each one changes and why. Don't change anything yet.
 ````


````text
review the whole solution like a reviewer would. Give me a list of findings with file, problem and suggested fix, but don't change anything yet.
Check:
- architecture and project references
- correctness and edge cases
- naming and readability
- tests
- UI and API
````

````text
implement that code should fall back to english for invalid language codes
````

````text
when another developer gets this code, will vs automatically select api and web projects as a start up projects?
````

````text
should we add Api integration tests? Give me your recommendation. what would be the cost and benefit of adding them?
If you recommend adding them, show me the plan first and we will implement it together.
````

````text
Do a security review of the whole solution before we write the README and host it. don't change any code, report only.
public web app, planned hosting(probably Render), blazor web assembly, .net core minimal api
check against owasp top 10, input handling and error handling, cors, dependencies, and possible configuration for production.
````

````text
Show me your recommended fix for s1 request size first, show me how to mitigate the 25 MB attack.
````

````text
Write README.md for this repo. Reader is a reviewer grading a coding task.
reading for about 5 minutes on GitHub.
Rules:
- Check every command, port, file path, number and example against the
  code, or run it. If you can't check something, ask me instead of writing it.
- Sections, in this order: what it does (2 lines + live link + screenshot),
  build and run, API, architecture, design decisions, input rules, tests,
  known limitations, deployment, adding a language.
- Input rules: one table, one row per rule with an example, including the
  rejected cases.
- Known limitations: use the list in memory. Don't describe any of them
  as fixed.
- No badges, emojis, "Features" list or closing summary.
````

