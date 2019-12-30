# ActivitySampling V1
A small .net core tool running in a console window.
It will ask the user every [xx] minutes
"What is the focus of your actual work" 
and store this information in a daily .csv file.

### Start parameter
    ActivitSampling.exe [Minutes]
The sampling rate can be defined in minutes as command
line argument. Normally 20 minutes are OK. If the argument is 
missing the program starts in demo mode and the question will
appear every 30 Seconds.

### Runtime behaviour
The App is asking in the middle of the sampling slot. So the
first question is triggered after [xx]/2 minutes (15 sec in demo mode).

The question is shown for 25 seconds.  

Every 5 seconds the PC will beep. If the user ignores the question
the default string "no activity" is shown on the screen. This "answer"
is not stored into the .csv file.

If the user press a key while the question is active the countdown 
is stopped. Now the User can type in the actual activity. 

### Configuration parameters
The follwing parameters are defined in the configuration file
`ActivitySampling.json` :
- the culture (`de-De`, `en-US`, ...) 
- the Time to Answer (in seconds)
- the base path of the data directory, default is the application folder (`.\`)
- the file extension of the data file, ending with .csv is default

### Localization
The Programm is already localized for english and german culture.  
To add another culture just add the resource file.

---

(c) 2019 by Christof Konstantinopoulos

>[Based on an idea of Ralf Westphal 
(https://ralfw.de/2017/08/zeiterfassung-mit-activity-sampling/)](
https://ralfw.de/2017/08/zeiterfassung-mit-activity-sampling/)
