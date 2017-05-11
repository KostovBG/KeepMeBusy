# KeepMeBusy
## Introduction
Keep Me Busy is a small program that simulates busy state so that the computer does not lock your session and you appear as "Active" in all Instant Messaging applications.

## How It Works
The application has two basic modes for simulating busy state - setting the thread state to busy and simulating a simple keyboard interaction.

### Simulate Busy State
Signals the OS that it shouldn't turn off the display or start the screen saver.
The behavior is the same as if you are watching a movie in full screen.
If you enable this setting on a Remote Desktop server, your session will not be locked.

### Refresh state every minute
Simulates a key press every minute. 
This option is useful with some IM programs like Skype for Business when you want to be seen as "Available" all the time.

To avoid messing up your work with additional keyboard interactions, the program opens a small window in the top left portion of the screen and sends the keystroke there. 
Since the active window is switched for a fraction of the second, you may notice that your currently active program flickers from time to time.

## Contacts
This project is currently maintained by Konstantin Kostov (konstantin at kostov dash bg dot com).
