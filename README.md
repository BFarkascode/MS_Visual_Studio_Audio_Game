# MS_Visual_Studio_Audio_Game
Building a small game to guess a song.

## General description
We will be building an application in Windows using C#. This application will be using a canvas/app window, some basic controls (buttons, drop downs, scroll bars, sliders) and the windows media player widget. We will connect the buttons up in a way that they will be controlling the media player playing back short audio snippets, resulting in a sound-based guessing game.

No secrets, this will be a relatively simple project here. Generally, I will mostly be touching upon the particularities and the differences in thinking in this repo vis-à-vis regular C and C++ while also providing the coding and the skeleton of the app.

This project fits rather comfortably in my drive to pivot towards more serious operating systems without breaking my brain on the learning curve, so bear with me.

### What is C#?
Something that may not be clear from the get-go – or at least it wasn’t for me – is the question of C#. For some time, I actually thought it was an alternative way to write C++…which is conceptually wrong, but not in the practical sense.

In reality, C# is Microsoft’s twist on C++. It is not C++, but close enough that we may consider them to be practically the same regarding philosophy and syntax. There is one relatively big difference though: it is completely string-based. Most data flows will be done using strings, variables will be strings, visual elements (buttons) will be represented by strings. We will thus have to convert any non-string data into strings when processing them as well. (Luckily, there is the “ToString()” function built-in to do that for us.) We will also have to use structs/classes to handle the data a lot.

But, why bother? Why not just use C++? Simple: C# is full of libraries that allow us to directly access resources in Windows. As such, we can easily generate applications that will be running on that operating system. More precisely, C# is organized in the .NETFramework which provides (or, at least, it used to) the backbone of Windows.

To be fair, I am rather sure that there are better, more efficient ways to make an app or even use Java to do the coding and then let it be completely web-based. For me though, I wanted to stay localised and not learn anything too new just yet, thus the legacy solution.

### What is Visual Studio?
Visual Studio is one of the programming environments in Windows. It should NOT be confused with Visual Studio Code, which is the go-to IDE for programming with plugins to make it compatible with any type of programming language.

No, Visual Studio is something different. First, as far as I can tell, it is only compatible with C#. Secondly, it has a visual element where we are filling a canvas/window/app environment up with elements, the elements with events, and then the events with code. Lastly, the most important way to tell the difference is that the icon of Visual studio Code is blue, while Visual Studio is purple…

Jokes aside, we will be constructing something visually first in Visual Studio and then fill up each of these visuals with the desired activity. Think of interrupt handlers but instead of having a trigger attached to a timer or an external physical stimulus – i.e. physical button press – the trigger will be attached to a change within the visuals of our app – clicking a button, moving a scroll bar, dropping down a menu and so on.

I must say, Visual Studio is quite outdated regarding design so if someone wants to go fancy, this definitely isn’t the way to go. For me though who prefers functionality to bling, it is a good bare bones solution to the app generation problem.

(Here I must mention that I am using Visual Studio 2022 to construct the app, which is at the time of writing is already considered obsolete and replaced by Visual Studio 2026 or some “AI” tool I have already forgotten about. Considering the documentation I have read, this likely means that the code I am providing below will not work with newer Visual Studio versions. The media player section especially seems like it had a complete rework in the 2026 version.)

Anyway, what bits/files will we be modifying within Visual Studio?

#### Auto-generated files
These will be the Cmake or bash files for the application (such as “program.cs” or “App.config”). They will define the name of the app and then carry on calling the definition functions of the app followed by the main class of the app. These files are all generated automatically by Visual Studio and until this day, I have yet to see any point modifying them.

#### App’s “.cs” file
This file is going to be our “main” code…but unlike before where we had setups and loops, we will have a “namespace” that calls functions and classes (yes, like in C++, we will be running classes for EVERYTHING). The classes will then be called or activated depending on how the app is being set up. A good example is the call of the base class (initially done in “program.cs” by the way) which is done by the app when you start running the “exe” file on your system. It will do an initialisation of all the variables we have defined in the namespace plus all the components we have activated on the canvas.

Functions are mostly callbacks of the elements put on the canvas of the app, albeit we can also add standalone functions here that will then be called by the callbacks. Of note, Visual Studio indicates how many times a function is referenced which helps cleaning up the code.

#### App’s “Designer.cs” file
This is the file that will store the visual element of the app, i.e. the app’s canvas. Generally, we will construct the visuals by dragging a dropping elements/widgets on the canvas from the ToolBox menu (which, if not opened already, can be done so by “CTLR+Alt+X”). We assign properties and events to these elements then by opening the “properties” menu (right click on the placed element and go to “Properties”). We can thus populate our canvas and assign properties visually and the “designer.cs” file will then be generated automatically by Visual Studio with the callbacks placed also automatically into the app “.cs” file.

By the way, the designer file itself is just code in C# describing the visuals. It is exactly the same code we can then modify using our app’s own “.cs” file, should we choose to do so.

#### App’s “.exe”
This is the built application, tucked away in the “bin/Debug” folder. Pretty much everything we have in that folder is what we will need to run our application. For simple apps, this will only be the “exe” file, i.e. that “exe” will be enough to share the app with users.

Unfortunately, at least in Visual Studio 2022, there may be some “trash” files around the “exe” that are not integrated into it”, like calling and calibrating the media player widget. I am not sure, why this is as such, but at any rate, one will have to include those files too when shared to make the app work.

#### Copying a project
Another thing to mention is how to properly copy a project. Yes, it is possible to simply CTRL-C+CTRL-V and then import the new copy as an existing project, but this does not change the aforementioned bash files (and where they point to) so we still technically will be mucking about in the original project most of the time.

For an actual copy to be done with a naming that serves our needs, one must generate a blank project with the desired name, then copy the “.cs”, “designer.cs”, “program.cs” and the “.csproj” files over from the original project. Once done, these files must be edited one by one using notepad or some other text editor to replaced the old project name with the new one, to change the project root and the project namespace. (Just search for the original name in the files and replace it with the new name.) If all goes well, this will merge the “blank” project bash files with the existing modified “old” design files to create the new project with the desired name but content from the copied project. 

#### Get used to some garbage
One last thing that must be pointed out: being already used to the top-notch Zephyr documentation where just by reading it you understand, how a function works, what it calls and with what variables, I was shocked to see how absolutely useless the Microsoft documentation is for C#. It does not explain much regarding functions, nor their variables, just sort of freeballs it and let you figure everything out on your own. It sometimes completely passes up the opportunity to explain, how to run a certain function at all and just gives you the name of the function. I am yet to figure out for instance, how to poll the state of the media player WITHOUT breaking the entire run of the app and the documentation was not useful at all on the matter.

Similarly, I noticed that while importing the media player is easy, making it work smoothly is not. I assume that putting an extra layer or two between yourself the outcome – i.e. Windows calling the app which then calls the media player widget which then calls the file to play – may add delays in the general execution that may break the run of the app, especially if the media outputs are very short and follow each other in a quick sequence. An example: playing back a string of audio snippets shorter than 0.2 seconds can often stutter the media player in the app, but not when we open them up using the media player directly.

## To read
We will be spending a lot of time on this, unfortunately:

[Windows Media Player Object Model | Microsoft Learn](https://learn.microsoft.com/en-us/previous-versions/windows/desktop/wmp/windows-media-player-object-model)

I also suggest giving this video a quick look since it gives a good summary over what we can do:

[Music Player in C# Visual Studio By Rohit Programming Zone](https://www.youtube.com/watch?v=QJkFfKDhz5o&t=1619s)

And this one is to show, how to control an Arduino with a C# app. Nice, basic stuff:

https://www.youtube.com/watch?v=1cQqYJCFm94&list=PLDxm-EGn62t7indrQcJGBchHJCJqTWdGP&index=2

## Particularities
Let’s discuss some of the particularities I have encountered while doing this project.

### Toolbox
When we open the toolbox, we will be greeted by a lot of different widgets and elements that we can import into our canvas. It is not a full list though. As a matter of fact, media player is often not included in the list at all, even though it is very often used in Visual Studio projects. (I do wonder why such omission…)

Anyway, we can add items to the ToolBox by right clicking on the list, select “Choose Item” and then find the desired app in the list that opens up. Of note, the media player will be a “COM component”.

Something to remember also that some tools will not be “part” of our canvas even if they are included within the app. For example, how the “Serial port” is handled is that it will not emerge as an item on the canvas but will be placed “outside” on a separate section - the section being hidden when empty. We will still be able to access it within the app though.

### Events
This is the big one and the one that took me some time to figure out: we can add events to the element on our canvas by manipulating them directly on the canvas (say, clicking on them) OR by going into “properties” of the element and then selecting the “lightning” icon instead of the properties icon on the menu header. There we will be greeted by a long list of potential actions that could befell the element and for each, we can define ourselves a callback function. In other words, we can manually select an event from this list and then tell the app, which function should be called when that event befalls the element.

The most common event we will be using is “click”, which, as the name suggest, will be called when we click on the element. Another common one will be “scroll” which will be called when we, well, scroll the element, say, a volume bar. We can also assign callbacks to events as we open the app or we close it.

Anyway, the point is that as we manipulate the canvas of the app, we will be having a conga line of callbacks activating under the hood. In other words, our thinking must be that we are uniquely and only using interrupts within our stripped-down code and we won’t have a “loop”.

Here I must note that we might have threads running in an app even without defining them. This I have not figured out yet…

### Modify properties
Every element on the canvas will be a separate class with parameters to be modified on the fly – which modifications may then cause an event, an event calling a callback and so on.

Not every property will be simple, like a string or a bool or an integer. For dropdowns for instance, the dropdowns will be “ComboItem” enums where the ID – i.e. the place – of the text will have to be defined alongside the text we want to show in the dropdown. 
An important modification to remember is that we can actually enable/disable an element on the canvas. When we disable an element, it will still be visible but greyed out, meaning that it will not work anymore. This is a critical capability to allow user progression through the app. I often also change the existing text on the disabled element – i.e. remove it – to reinforce the deactivation.

Lastly, if we have a text box in the app, we can generate a terminal window within our app by appending strings to it. We extract text from text boxes by simply reading them out. A test box in the app can be both something to write from and to publish data to, just like in the Arduino IDE. We can disable writing to the textbox by making its read only property “true”.

### Accessing PC resources
As mentioned, we will have access to the PC’s resources using C#. In this app, I am using our access to the file system of the PC to find where the app is stored (“Path.GetDirectoryName()” applied on the exe location “Assembly.GetEntryAssembly().Location”) and then import the paths for the audio snippet files (“Directory.GetFiles()”) which will then be attached to the buttons.

In another project, I was extracting local time from the PC by calling the “DateTime” built-in class.

Similarly, we could access the available COM ports by calling the right class function, given we have the SerialPort included on the canvas, obviously (“SerialPort.GetPortNames()” will give back the available port names as an array of strings).

We can use the Visual Studio terminal by running the “Console. Writeline()” function. The function’s input is a string.

### Mp3s
I am usually using Wav files in my projects since they are easy to manipulate with microcontrollers. When I generated the audio snippets for this app, I also did the same, only to realise that my snippets make the app’s media player stutter instead of playing back normally. It did work when played back from the media player directly though, so I was a bit baffled by the situation.

The solution to this bug had been to save the snippets in mp3 format instead of wav. Mp3s are significantly smaller files (they are compressed already compared to a Wav) and thus are easier to buffer by the player.

### Using playlists
I ended up using a playlist in my design to play back the snippets in the desired sequence since I could not make the media player play back one file and then wait until that playback is done before playing the next. I just could not figure out the events and the callbacks, nor could I specifically delay the execution of the code until the playback has concluded.

Also, I had to make the playlist first be full before I attempted a playback since doing it in the other way around meant that only the first snippet was played back and the rest discarded.

## User guide
The goal of the game is to reconstruct the original audio from smaller audio snippets by selecting from a “good” or a “bad” snippet using the buttons.

Independent of if the player has managed to reconstruct well the snippet sequence or not (and thus remake the original audio), the validation button will play back their selection.

If successful, the original audio is played back plus a text is shown in the text box (say, the name of the guessed song).

The “Aid” button is there to play back the original audio file to help with selecting the right sequence.

Progress through the game is shown in the score section.

The volume of the output is controlled with the scroll bar in the app.

There is a dropdown menu to select from “Beginner” and “Advanced” mode.

(Just a note here, the difficulty curve is not smooth between the two in the slightest. Frankly, I made the “Advanced” level for the coding fun of it, I doubt anyone would find it entertaining. It is simply too difficult.)

### How to set up the app for your personalised game
Download the entire app folder as one block. The app will start by clicking on the “exe” file. No need to install anything.

The audio file folder will have already multiple folders which will need to be filled up by audio files (a few dummy audio files are shared). In the shared version, there are 10 folders each in both the “Beginner” and the “Advanced” folders, though these can be extended by the user if the user respects the existing naming convention (i.e. give the same name to the new folders with ascending numbering).

Please note that the app is looking for 10 tests, so it won’t work with less than 10 folders. The shared folders are filled up by placeholder audio snippets. The app will be playable but not enjoyable without setup.

To generate the audio snippets, I recommend using Audacity. It is a free and easy to use program to take an audio file and then chop it up into snippets.

Before being placed into their respective folders, the snippets must be named as “1_1A.mp3” for the first “correct” snippet and “1_1B.mp3” for the first “wrong” snippet. The naming must be adhered to where the first number is the number of the audio folder, the second the number of the snippet and the letter “A” and “B” to indicate the property of the snippet. Say, if the original sound is a sentence, first number is the number of the folder where we place the snippets, the second is the place of the word within the sentence stored within the snippet) and the letter will be define if it is the correct snippet or the wrong one.

The original audio to be guessed should be saved as the number of the audio folder followed by the word “solution”. The files must be stored in the same folder as the snippets.

The game will automatically – and randomly - open the audio files from these folders. There is no limit on the potential number of audio tests, just create new folders as demanded and the app will randomly select from them 10 during execution.

There is also the "solutions” text file within the folders. This is from where the solutions text box will be filled up. One must add here a comment for each audio file since this “solutions” text file is used to store the number of potential tests to select from. One should always have at least empty lines added to that file when expanding the audio library with new tests. (The app checks the number of lines in the file to know the number of potential tests. Clumsy, I know, but works.) There is a separate “solutions” text file for the “beginner” and the “advanced” mode.

Lastly, a small word of advice: if one wishes to use the app to cut an audio flow up (say, a sentence word by word), the generated snippets must remain still long enough to be audible. What I mean is that short words taken out of context and then left by themselves – i.e. not padded out by adequate pauses and not being articulated well – become inaudible gibberish when turned into a snippet.

### How to play - Beginner level
Select the “beginner” difficulty from the dropdown menu and click “Start”. We will be guessing the audio files in the “Beginner” folder. Audio files in the “Beginner” folder are intended to be shorter (less snippets per audio).

The buttons are in ascending sequence, i.e. the first good/bad snippet pair is assigned to the first pair of buttons.

One can use the “Aid” button to play back the original audio any number of times.

One can try until successfully.

After 10 steps, the game ends.

### How to play - Advanced level
Select the “advanced” difficulty from the dropdown menu and click “Start”. We will be guessing the audio files in the “Advanced” folder now. Audio files in the “Advanced” folder are intended to be longer (more snippets per audio).

The buttons are now random in sequence, meaning that the first snippet pair may not be assigned to the first button pair at all. The sequence of the selection is stored within the central text boxes instead which must be defined for each snippet pair now. An undefined position is marked by a “?”.

The “Aid” button can only be used three times.

Validation will only proceed if there is no undefined position left. Validation will not proceed is the same position is used multiple times. One can try to validate as many times as desired.

There is a “Pass” button which passes the phrase and goes to the next one. Passing will introduce a pass counter just below the score board. The score board will not be updated when we pass.

After 10 steps, the game ends.

## Conclusion
This is just a fun little project to discuss my findings while playing around with Visual Studio.

I have decided to share in the hopes that someone will have fun with the app in the future.
