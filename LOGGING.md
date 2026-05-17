# Logging Strategy

## Purpose

The application uses console logging to help with:

- finding problems
- tracking errors
- checking the environment
- understanding what the application is doing

## Logging Rules

- Never log sensitive information such as:
  - passwords
  - connection strings
  - tokens
  - personal user data

- Use the correct log level:
  - Debug: extra details for development
  - Information: normal application events
  - Warning: something unexpected happened, but the app still works
  - Error: something failed

## Environment Settings

- Development:
  - minimum log level = Debug

- Production:
  - minimum log level = Information

## Current Logging

The application currently logs:
- application startup
- current environment
- unexpected errors

## Output

Logs are written to the console with timestamps.