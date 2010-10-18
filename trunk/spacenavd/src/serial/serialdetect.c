/*
serialdetect - detecting serial spaceball/magellan device.
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

#include "serial/serialdetect.h"


int detectDevice(char *devFile, char *buffer, int bufferSize)
{
  if (openFile(devFile) == -1)
    return -1;
  
  return 0;
}

int openFile(const char *devFile)
{
  return open(devfile, O_RDWR | O_NOCTTY | O_NONBLOCK);
}

int setPortSpaceball(int fileDescriptor)
{
  
}

int setPortMagellan(int fileDescriptor)
{
  int status;
  struct termios term;
  tcgetattr(file, &term);
  
  term.c_cflag = CS8 | CSTOPB | CRTSCTS | CREAD | HUPCL | CLOCAL;
  term.c_iflag |= IGNBRK | IGNPAR;
  term.c_oflag = 0;
  term.c_lflag = 0;
  term.c_cc[VMIN] = 1;
  term.c_cc[VTIME] = 0;
  
  cfsetispeed(&term, 9600);
  cfsetospeed(&term, 9600);
  tcsetattr(file, TCSANOW, &term);
  
  if (ioctl(file, TIOCMGET, &status) == -1)
    return -1;
  status |= TIOCM_DTR;
  status |= TIOCM_RTS;
  if (ioctl(file, TIOCMSET, &status) == -1)
    return -1;
  return 0;
}
