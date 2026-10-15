# NoteTakingAPI
Backend application for a note-taking system. Supports notes stored as simple text strings.

Stores and retrieves notes by internal id, owner, and associated team name.

## What is in scope?
- The data model for a note.
- Access methods for notes
    - By NoteId
    - List of Notes by OwnerId
    - List of Notes by TeamName
- Database access layer for Note objects.
- Simple error cases with HTTP status codes.
- Tests with mocks for each endpoint.

## What is out of scope?
- UI for viewing/modifying Note objects.
- An actual database table.
- Models for Users/Owners/Teams
- Authorization/Authentication

## Design Decisions 
### What database technology should we use?
The notes in this application are strings with decorating attributes for tracking. There are no data models for users, owners, or teams. 
Because there is no need for an SQL solution when working with these simple objects, we will use a NoSQL solution. 
It must be queryable by multiple fields, so a simple document database like MongoDB is not optimal.
AWS DynamoDB supports scanning and querying by multiple fields without SQL, so we will use that.

However, due to the timed scope and the fact that I do not want to share AWS credentials, I will be backing the sample db from a dictionary.

### What data should a note contain?
A note object in this project is going to be considered an unstructured blob of text with several additional fields.
Because the notes should be accessible by users who are not the owner, we will add a team name field that can be used to view a list of notes from multiple users.
The note will contain OwnerId. This is because a user would likely want access to a list of their own notes.
Finally, there will be a title. The title is used as a secondary key to distinguish notes from one another.
There is no need to have a unique ID for the note. It will be cardinal based on title and ownerId

## Future Opportunities
- Add authentication and authorization to make sure that the current user has access to the requested notes.
- Write the connector code for a real DynamoDB connection. It is omitted here because I don't have a real remote database.
- Add additional unit tests for all the controller methods. I only had time for a simple GET test.