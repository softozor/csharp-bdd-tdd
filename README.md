# .NET BDD / TDD

The purpose of this repository is purely didactical. You can already access [our introductory presentation to BDD / TDD](unit-testing.pptx). A live coding video will soon arise from the code published here. The idea is to present minimal code featuring behavior- and test-driven development when producing a new [Prism](https://prismlibrary.github.io/docs/) module in the .NET framework.

We are implementing [one single requirement](after/Spec/Features/PersonsManagement.feature) that will be the only acceptance test. Only the necessary model and view model are developped, no view. First, we explain how to construct our acceptance test with [SpecFlow](https://specflow.org/getting-started/). Then we explain how to develop the required module following TDD. 