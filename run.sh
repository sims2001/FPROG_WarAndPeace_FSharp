#!/bin/bash

cd WarAndPeaceTests

dotnet test 

cd ..

dotnet build WarAndPeace
dotnet run --project WarAndPeace --configuration Release