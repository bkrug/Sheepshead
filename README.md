Sheepshead
==========

Use this applicaiton for 3 or 5 player Sheepshead games with up to five of them being human.
See the deployed app at www.simplesheepshead.com

The startup project within the solution is "Sheepshead.React"
* Sheepshead.React - uses the React framework
* Sheepshead.Logic - has game logic in it and would be usable with a different front-end framework
* Sheepshead.Tests - all tests are run on Sheepshead.Logic
* Sheepshead - this project is meant to be a javascript-light MVC version of the application. For the moment it is not included in the main solution file. I wrote this version before the Sheepshead.Logic project was moved to .NET Core, and I haven't yet decided if I will convert the javascript-light version of the appliation.

Work to do:
* When a user buries cards, the front-end thinks that they can bury a thrid card. Fix this.
* Give users a way to change their mind after selecting a card to bury. Right now the user would have to refresh the page.
* Handle Leasters games that tie.
* Get more feedback from non-programers who now Sheepshead well.