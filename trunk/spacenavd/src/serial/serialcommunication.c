 /*
 serialcommunication - serial communication for spaceball/magellan device.
 Copyright (C) 2010 Thomas Anderson <ta@nextgenengineering.com>
 
 This program is free software: you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation, either version 3 of the License, or
 (at your option) any later version.
 
 This program is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.
 
 You should have received a copy of the GNU General Public License
 along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */  

 #include <fcntl.h>
 #include <sys/ioctl.h>
 #include <stdio.h>
 #include <termios.h>
 #include <unistd.h>
 #include <errno.h>
 #include <string.h>
 #include "serial/serialconstants.h"
 #include "serial/serialcommunication.h"
 
 int openFile(const char *devFile)
 {
   return open(devFile, O_RDWR | O_NOCTTY | O_NONBLOCK | O_NDELAY);
 }
 
 int setPortSpaceball(int fileDescriptor)
 {
   struct termios term;
   if (tcgetattr(fileDescriptor, &term) == -1)
   {
     printf("error tcgetattr: %s\n", strerror(errno));
     return -1;
   }

   term.c_cflag = CREAD | CS8 | CLOCAL | HUPCL;
   
   term.c_iflag |= IGNBRK | IGNPAR;
   term.c_oflag = 0;
   term.c_lflag = 0;
   term.c_cc[VMIN] = 1;
   term.c_cc[VTIME] = 0;
   
   cfsetispeed(&term, 9600);
   cfsetospeed(&term, 9600);
   if (tcsetattr(fileDescriptor, TCSANOW, &term) == -1)
   {
     printf("erro tcsetattr: %s\n", strerror(errno));
     return -1;
   }
   
   int status;
   if (ioctl(fileDescriptor, TIOCMGET, &status) == -1)
   {
     printf("error TIOCMGET: %s\n", strerror(errno));
     return -1;
   }
   status |= TIOCM_DTR;
   status |= TIOCM_RTS;
   if (ioctl(fileDescriptor, TIOCMSET, &status) == -1)
   {
     printf("error TIOCMSET: %s\n", strerror(errno));
     return -1;
   }
   
   return 0;
 }
 
 int setPortMagellan(int fileDescriptor)
 {
   int status;
     
   if (setPortCommon(fileDescriptor, CS8 | CSTOPB | CRTSCTS | CREAD | HUPCL | CLOCAL) == -1)
     return -1;
   
   if (ioctl(fileDescriptor, TIOCMGET, &status) == -1)
     return -1;
   status |= TIOCM_DTR;
   status |= TIOCM_RTS;
   if (ioctl(fileDescriptor, TIOCMSET, &status) == -1)
     return -1;
   return 0;
 }
 
int setPortCommon(int fileDescriptor, int flags)
 {
   struct termios term;
   if (tcgetattr(fileDescriptor, &term) == -1)
     return -1;
   
   term.c_cflag = flags;
   term.c_iflag |= IGNBRK | IGNPAR;
   term.c_oflag = 0;
   term.c_lflag = 0;
   term.c_cc[VMIN] = 1;
   term.c_cc[VTIME] = 0;
   
   cfsetispeed(&term, 9600);
   cfsetospeed(&term, 9600);
   if (tcsetattr(fileDescriptor, TCSANOW, &term) == -1)
     return -1;
   return 0;
 }
 
 void longWait()
 {
   usleep(150000);  
 }
 
 void shortWait()
 {
   usleep(2000);  
 }
 
 void serialWriteString(int fileDescriptor, char *string, int count)
 {
   int index;
   for (index=0;index<count;++index)
   {
     write(fileDescriptor, string + index, 1);
     shortWait();
   }
   write(fileDescriptor, "\r", 1);
   longWait();
 }
 
 int serialRead(int fileDescriptor, char *buffer, int bufferSize)
 {
   int bytesRead, index;
   bytesRead = read(fileDescriptor, buffer, bufferSize-1);
   if (bytesRead<1)
   {
     printf("no bytes read\n");
     return 0;
   }
   buffer[bytesRead] = '\0';
   
   for (index=0;index<bytesRead;++index)
   {
     if(buffer[index] == '\r')
       buffer[index] = '\n';
   }
   
   printf("%s\n", buffer);
   return bytesRead;
 }