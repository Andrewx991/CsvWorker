# How-To

1. Download and install .Net Core 2.0.2 -> https://www.microsoft.com/net/download/windows
1. cd to `..\CsvWorker\src\CsvWorker`
1. Run dotnet restore
1. Run dotnet build
1. Run dotnet run .\CsvWorker.csproj -i C:\INPUT -o C:\OUTPUT -e C:\ERROR
1. Program will poll for new files every 5 seconds.
1. Ctrl+C to terminate.

# Known Issues

1. Not using a standard csv parsing library, so no comma escape support.
1. If forcibly terminated during processing output files may be invalid.
1. Did not implement field specific validation error messages.
1. Directory monitoring could be improved to hook into file system events rather than polling, but not sure how that differs on non-Windows environments, so opted for simpler implementation for now.
1. State is lost if program terminated and re-run.

# Assumptions

## How to handle files its already seen

Requirements stated to only process new files.
New files were defined as the name was not yet observed.
Requirements also stated in the event of name collisions take the newest.
These conflicted, made the assumption that we shouldn't overwrite the output from the original file and ignore the new version because it is not 'new' as defined in requirements.

## How to handle non-standard headers

In the event the headers are not as expected it will log an error but proceed to try and process the file.

## Empty Files

Assumed files can be empty, if so still output an empty array in the output file.

## Non-Functional Requirements

No specific performance or environment requirements were provided. Made the following assumptions:
- Underlying filesystem behavior can be ignored (max number of files, file size limits, IO performance)
- No need for multi-threading at this time.
- No need for time limits per file.

## This solution does not have to account for reprocessing files once fixed

Assumed that this system doesn't need to handle re-parsing a corrected file and making sure the output file is updated/corrected. Also assumed we should not delete files that have only been partially processed.

## Did not design to be used generically

Despite headers being provided with the csv, the output structure still requires the code to be aware of specific field name, relationships, and validation rules.