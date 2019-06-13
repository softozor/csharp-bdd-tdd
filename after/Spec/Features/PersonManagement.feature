Feature: Technical Officer manages persons
  As a Technical Officer or an Instructor
  I want to manage the list of the people using the simulator
  in order to produce access badges.

Background: The database is filled with persons
  
  Given a list of persons was persisted to the database 

Scenario: The Technical Officer manually persists a new person to the database

  In addition to importing person data from a file, the Technical Officer can 
  add persons to the system manually. First, the Technical Officer needs to 
  add a new person. That person is then stored in the application. The Technical 
  Officer finally needs to save in order to definitely persist the person to 
  the database. 

  Given the Technical Officer has added a new person
  When she saves
  Then the new person is persisted to the database 

Scenario: The Technical Officer is browsing through the persons' list

  Then she has access to the persisted persons

Scenario: The Technical Officer imports new persons

  Pure data importation has no effect on the underlying database's state.

  When the Technical Officer imports a list of persons 
  Then they are not persisted to the database