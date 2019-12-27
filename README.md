# ActivitySampling
A small .net core tool running in a console window.
It will ask the user every xx minutes "What is the focus of your actual work" 
and store this information in a daily .csv file. 

The sampling rate can be defined in minutes as command line argument. Normally 20 minutes are OK.
If the argument is missing the program starts in the demo mode and the Qustion appears every 30 Seconds. 

The question is shown for 30 seconds.
If no answer is given from the user the default string "no activity" is used.
If the user press a key the countdown is stopped. Now the User has more time to type in the actual activity.


Based on an idea of Ralf Westphal (https://ralfw.de/2017/08/zeiterfassung-mit-activity-sampling/)

(c) 2019 by Christof Konstantinopoulos
