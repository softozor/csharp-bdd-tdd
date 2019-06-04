Feature: Technical Officer manages persons
	As a Technical Officer or an Instructor
  I want to manage the list of the people using the simulator
  in order to produce access badges.

  The person data are stored in a database which can consist of a local file 
  or a more sophisticated system (e.g. oracle database) depending on the use 
  cases this feature needs to address.

Background: The Technical Officer is browsing through the persons' list
  
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

Scenario: The Technical Officer imports new persons

  Pure data importation has no effect on the underlying database's state.

  When the Technical Officer imports a list of persons 
  Then the new persons can be seen in the view 
  But they are not persisted to the database

Scenario: The Technical Officer makes the imported persons persistent

  Upon saving, the database is affected by the changes to the persons' data.

  Given the Technical Officer imported a list of persons
  When she saves them 
  Then the new persons are added to the database

Scenario: The Technical Officer exports the whole list of displayed persons

  Upon persons' list exportation, all the persons displayed in the list 
  are exported, which might not correspond to the database's current state. 
  It can indeed be that the Technical Officer updated the original database 
  data before exporting.

  Given the Technical Officer imported a list of persons
  When she exports all the persons to a local file
  Then the local file is filled up with the details of all the persons depicted in the view

Scenario: The Technical Officer exports a selection of displayed persons

  It is totally possible to export data that is not present in the database. 
  The Technical Officer can indeed load the persisted data, add a new person, and 
  only export that latter person. 

  Given the Technical Officer has selected a few persons
  When she exports them to a local file
  Then the local file is filled up with the selected persons' details
  
Scenario: The Technical Officer deletes a person from the view

  Again, deleting the person from the view doesn't affect the database at all. 
  The changes need to be saved for that to happen.

  Given the Technical Officer has selected a person 
  When she deletes her
  Then the person disappears from the view 
  But it is still present in the database 

Scenario: The Technical Officer deletes a person permanently

  Given the Technical Officer has deleted a person 
  When she saves
  Then the person is not in the database anymore 

Scenario: The Technical Officer deletes a list of persons from the view

  Given the Technical Officer has selected a few persons
  When she deletes them
  Then the persons disappear from the view 
  But they are still present in the database

Scenario: The Technical Officer deletes a list of persons permanently

  Given the Technical Officer has deleted a few persons
  When she saves 
  Then the persons are not in the database anymore 

Scenario: The Technical Officer generates access badges for the whole list

  The Technical Officer gets her list of persons from her hierarchy. 
  The data come from the "Personalinformationsystem der Armee" (PISA). 
  Out of her list, the Technical Officer can generate a badge for each 
  person. A badge exhibits the person's rank, first and last names, the school's 
  insignia and the QR code. Note that the list might not completely represent 
  the database's state. 

  ![Sample badge](img/sample_badge.png)

  When she prints badges for everyone in the list
  Then she gets a PDF file with all the badges

Scenario: The Technical Officer generates access badges for a selection of persons

  The Technical Officer can select a few persons to generate their badges.

  When the Technical Officer selects a few persons
  And prints badges for them 
  Then she gets a PDF file with the badges of the selected persons

# TODO: in the future, deal with the case of concurrent accesses to the same piece of data
# TODO: allow for persisted persons edition?