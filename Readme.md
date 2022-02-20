# Goal

Make reservations for theater seats.

# Try it

- WebGL: [TODO: link to gh-pages]
- Desktop: [TODO: link to zip of *.exe within gh releases]

# Steps

1. Open the weblink or the *.exe
2. Choose name you'd like seats to be reserved under. If you previously reserved seats, those movies are listed at the top.
3. Choose day you'd like to reserve seats for a new movie.
4. Choose movie and time you'd like to see it that day.
5. Click to toggle your reservation for seats. Seat colors are as follows:
    |     |     |
    | --- | --- |
    | White | Available to be reserved. |
    | Green | Already reserved by you. |
    | Red | Reserved under a different name. You can not modify this seat. |
6. Click "Finished" button to be returned back to the "Choose name" page.

# Requirements

1. Seat reservations need to persist and be stored in the cloud. Across all users and all app sessions. Maybe google sheets for the backend?
2. Unit Tests used for checking ability to pull and storing mock data.

# Concerns

1. Someone can remove someone elses reservations by choosing the same name to use to reserve seats.
2. If internet is currently unavailable, there is no fallback to temporarily save locally or queue reservations for when internet is restored.

# Decisions

### Backend

I chose Google Sheets for the backend because I expect the app to not have more than 1 or 2 concurrent users. Google Sheets should provide the performance I need for those users.
I chose a scriptable object Google Sheets as the 

# Repository

[TODO: somewhere on gh]
