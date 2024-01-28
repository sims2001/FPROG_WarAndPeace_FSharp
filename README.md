# WarAndPeace Project

This program categorizes chapters of a large text file (e.g., "War and Peace" by Tolstoy) into either war-related or peace-related based on the occurrences of specific words. The program utilizes two additional text files containing word lists: one for "war-terms" and another for "peace-terms."

## How to Run

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) installed on your machine.

### Running Unit Tests

Navigate to the WarAndPeaceTests directory and run the following command:

```bash
dotnet test
```

### Running the Project

In this directory execute one of the following commands:

```bash
dotnet build WarAndPeace
dotnet run --project WarAndPeace --configuration Release
```
or
```bash
./run.sh
```

## Input Files

Place your text files in the same folder as your solution file. Ensure that your code reads the files using relative paths or adjust the working directory appropriately in your code.

## Contributors
- Rutschka Simon
- Wunsch Lukas

