# FileWatcher
Command line program to watch folder for added/deleted/modified files using a specific pattern

For this, I used the following acceptance criteria:
+ The program takes 2 arguments, the directory to watch and a file pattern, example: program.exe "c:\file folder" *.txt
+ The path may be an absolute path, relative to the current directory, or UNC. The target directory will have no subdirectories. 
+ The file system will be either NTFS or exFAT.
+ Use the modified date of the file as a trigger that the file has changed.
+ Check for changes every 10 seconds.
+ When a file is created output a line to the console with its name and how many lines are in it.
+ When a file is modified output a line with its name and the change in number of lines (use a + or - to indicate more or less).
+ When a file is deleted output a line with its name.
+ Renamed files can be treated as a delete and add.
+ Treat file names as case insensitive, e.g. files renamed only by changing case should be treated as no change.
+ Files will be ASCII or UTF-8 and will use Windows line separators (CR LF).
+ Multiple files may be changed at the same time, can be up to 2 GB in size, and may be locked for several seconds at a time.
+ Use multiple threads so that the program doesn't block on a single large or locked file.
+ There may be as many as 100,000 files at any given time.
+ Program will be run on Windows 10.
