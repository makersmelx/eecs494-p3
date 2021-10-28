# eecs494-p3

## Game Dev Note
### Controls:

1. WASD: basic movement
2. Mouse: perspective 
3. Space: jump
4. Left shift: grab the ledge (when player is close to it)
5. Left ctrl: crouch (something is wrong with this functionality, left to be modified)

### Flaws:

1. By now, it is not fun to play.
2. Controls:
   a) Mouse controling of camera seems to be not very comfortable.
   b) Camera is too easy to be inclined.
   c) Camera may be locked in some case like ledge
   d) Crouch is not correctly implemented.
   
## For development
A good practice is that for one feature that we propose, create just one issue on Jira, and create just one pull request for that on Github.
### Commit
A commit message should be structured as follow:
~~~
<type>(optional scope): <description>

[optional body]

[optional footer(s)]
~~~
1. Type of the commit includes `feat` (feature), `fix` (fix bug), `refact`(refactor the code), `doc` (doc update including research or txt), `misc` (other things like removing useless file) and others if needed
2. I suggest we use the issue number as the commit scope
3. Body and footer is optional

An example is
~~~
feat(#2): implement the control with keyboard input
~~~

Check [Conventional Commit](https://www.conventionalcommits.org/en/v1.0.0/) for details

### Create a new branch and pull request
Starting from some periods of the project, the master branch will be protected from direct commit. To commit a change to the master branch, create a new branch, write your code there and then make a pull request from the branch. A good practice is that for one feature that we propose, create just one issue on Jira, and create just one pull request for that on Github.
1. I suggest that we create the new branch with the naming as [name]/#[issue id], like `jiayao/#2` as the branch for the 2nd issue.
2. When creating the pull request,
   - include the issue id in jira, named the pull request with the feature, like `#1: set up the project management`
   - do not forget to **request view** from other of us

