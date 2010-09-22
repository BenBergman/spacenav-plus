/*
smag - decoding serial magellan spaceball data.
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


#include <stdio.h>
#include <fcntl.h>
#include <termios.h>
#include <unistd.h>
#include <sys/ioctl.h>
#include <string.h>
#include <serial/smag.h>

/*
v  MAGELLAN  Version 6.60  3Dconnexion GmbH 05/11/01
*/									/* magellan spacemouse plus xt serial. after sending vQ\r */


/*
v  MAGELLAN  Version 5.79  by LOGITECH INC. 10/10/97
*/									/* magellan spacemouse. after sending vQ\r */


#define MAXPACKETSIZE 16
#define MAXREADSIZE 64		/*version string is longer than packets*/

int file;
struct InputStruct
{
  char readBuf[MAXREADSIZE];
  int readBufSize;
  char packetBuf[MAXPACKETSIZE];
  int packetBufPosition;
} input;

int firstByteParity[16]={
  0xE0, 0xA0, 0xA0, 0x60, 0xA0, 0x60, 0x60, 0xA0, 0x90, 0x50, 0x50, 0x90, 0xD0, 0x90, 0x90, 0x50
};

int secondByteParity[64]={
  0x80, 0x40, 0x40, 0x80, 0x40, 0x80, 0x80, 0x40, 0x40, 0x80, 0x80, 0x40, 0x80, 0x40, 0x40, 0x80,
  0x40, 0x80, 0x80, 0x40, 0x80, 0x40, 0x40, 0x80, 0x80, 0x40, 0x40, 0x80, 0xC0, 0x80, 0x80, 0x40,
  0xC0, 0x80, 0x80, 0x40, 0x80, 0x40, 0x40, 0x80, 0x80, 0x40, 0x40, 0x80, 0x40, 0x80, 0x80, 0x40,
  0x80, 0x40, 0x40, 0x80, 0x40, 0x80, 0x80, 0x40, 0x40, 0x80, 0x80, 0x40, 0x80, 0x40, 0x00, 0x80
};

int open_smag(const char *devfile)
{
  if (open_file(devfile) < 0)
    return -1;
  setup_port();
  initMagellan();
  clearInput();
  return 0;
}

int close_smag()
{
  myWriteString("l000", 4);
  close(file);
  return 0;
}

int read_smag(struct dev_input *inp)
{
  /*need to return 1 if we fill in inp or 0 if no events*/
  int bytesRead;
  bytesRead = myRead();
  return 0;
}

int get_fd_smag()
{
  return file;
}

int open_file(const char *devfile)
{
  file = open(devfile, O_RDWR | O_NOCTTY | O_NONBLOCK | O_SYNC);
  if (file < 0)
    return -1;
  return 0;
}

void setup_port()
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
    printf("error TIOCMGET");
  status |= TIOCM_DTR;
  status |= TIOCM_RTS;
  if (ioctl(file, TIOCMSET, &status) == -1)
    printf("error TIOCMSET");
}

void get_version_string(char *buffer, int buffersize)
{
  myWriteString("\r\rm0", 4);
  myWriteString("", 0);
  myWriteString("\r\rm0", 4);
  myWriteString("c03", 3);
  myWriteString("z", 1);
  myWriteString("Z", 1);
  myWriteString("l000", 4);
  myRead();/*to read any pending port data. device likes to echo back commands.*/
  myWriteString("vQ", 2);
  if (myRead() > 0 && buffersize > input.readBufSize){
    strcpy(buffer, input.readBuf);
  }
}

void longWait()
{
  usleep(150000);  
}

void shortWait()
{
  usleep(2000);  
}

void myWriteString(char *string, int count)
{
  int index;
  for (index=0;index<count;++index)
  {
    write(file, string + index, 1);
    shortWait();
  }
  write(file, "\r", 1);
  longWait();
}

int myRead()
{
  int bytesRead;
  bytesRead = read(file,input.readBuf,MAXREADSIZE-1);
  if (bytesRead<1)
    return 0;
  input.readBuf[bytesRead] = '\0';
  input.readBufSize = bytesRead;
  return bytesRead;
}

void initMagellan()
{
  myWriteString("", 0);
  myWriteString("\r\rm0", 4);
  myWriteString("pAA", 3);
  myWriteString("q00", 3);	/*default translation and rotation*/
  myWriteString("nM", 2);	/*zero radius. 0-15 defaults to 13*/
  myWriteString("z", 1);	/*zero device*/
  myWriteString("c33", 3);	/*set translation, rotation on and dominant axis off*/
  myWriteString("l2\r\0",4);
  myWriteString("\r\r", 2);
  myWriteString("l300", 4);
  myWriteString("b9",2);	/*these are beeps*/
  myWriteString("b9",2);
  myRead();/*to read any pending port data*/
}

void clearInput()
{
  memset(input.readBuf, 0x00, MAXREADSIZE);
  input.readBufSize = 0;
  memset(input.packetBuf, 0x00, MAXPACKETSIZE);
  input.packetBufPosition=0;
}

void processDisplacementPacket()
{
  int index, lastBytes, offset;
  short int accumLast, number, accumLastAdjusted;
  accumLast = offset = 0;
  
  for (index=1; index<13; index+=2)
  {
    /*first byte check*/
    unsigned char low, up;
    low = input.packetBuf[index] & 0x0F;
    up = input.packetBuf[index] & 0xF0;
    if (up != firstByteParity[low])
    {
      printf("bad first packet\n");
      return;
    }
    
    /*second byte check*/
    low = input.packetBuf[index+1] & 0x3F;
    up = input.packetBuf[index+1] & 0xC0;
    if (up != secondByteParity[low])
    {
      printf("bad second packet\n");
      return;
    }
    
    number = 0x0000;
    number = (short int)((input.packetBuf[index] << 6 & 0x03C0) | (input.packetBuf[index+1] & 0x3F));
    if (number>512)
      number = number - 1024;
    accumLast += number;
    
    if (number<0)
    {
      offset--;
      offset += (int) (number+1)/64;
    }
    else
    {
      offset += (int) number/64;
    }
    printf("%8i ", number);
  }
 
  /*last byte of packet is a sum of 6 numbers and a factor of 64. use as a packet check
  still not sure what the second to last byte is.*/
  accumLastAdjusted = accumLast & 0x003F;
  accumLastAdjusted += offset;
  if (accumLastAdjusted < 0)
    accumLastAdjusted += 64;
  if (accumLastAdjusted > 63)
    accumLastAdjusted -= 64;
  
  lastBytes = (short int)(input.packetBuf[14] & 0x3F);

  if (accumLastAdjusted != lastBytes)
    printf("   bad");
  else
    printf("   good");
  
  printf("\n");
  return;
}

void processButtonKPacket()
{
  if (strcmp("k000", input.packetBuf) == 0)
  {
    printf("button released\n");
    return;
  }
  
  /*button pressed*/
  
  if (input.packetBuf[1] & 0x01)
    printf("button 1   ");
  if (input.packetBuf[1] & 0x02)
    printf("button 2   ");
  if (input.packetBuf[1] & 0x04)
    printf("button 3   ");
  if (input.packetBuf[1] & 0x08)
    printf("button 4   ");
  
  if (input.packetBuf[2] & 0x01)
    printf("button 5   ");
  if (input.packetBuf[2] & 0x02)
    printf("button 6   ");
  if (input.packetBuf[2] & 0x04)
    printf("button 7   ");
  if (input.packetBuf[2] & 0x08)
    printf("button 8   ");
  
  if (input.packetBuf[3] & 0x01)
    printf("button asterisk   ");
  if (input.packetBuf[3] & 0x02)
    printf("button left   ");
  if (input.packetBuf[3] & 0x04)
    printf("button right   ");
  
  /*printf("\n");*/
  printf("      %s\n", input.packetBuf);
}

void processButtonCPacket()
{
  /*these are implemented at device and these signals are to keep the driver in sync*/
  if (input.packetBuf[1] & 0x02)
    printf("translation is on   ");
  else
    printf("translation is off   ");
  
  if (input.packetBuf[1] & 0x01)
    printf("rotation is on   ");
  else
    printf("rotation is off   ");
  
  if (input.packetBuf[1] & 0x04)
    printf("dominant axis is on   ");
  else
    printf("dominant axis is off   ");
  
  printf("\n");    
  /*printf("%s\n", input.packetBuf);*/
}

void processButtonNPacket()
{
  int radius;
  radius = (int)input.packetBuf[1] & 0x0F;
  printf("zero radius set to %i\n", radius);
}

void processButtonQPacket()
{
  /* this has no effect on the device numbers. Driver is to implement any scale of numbers*/
  int rotation, translation;
  rotation = (int)input.packetBuf[1] & 0x07;
  translation = (int)input.packetBuf[2] & 0x07;
  printf("rotation = %i   translation = %i\n",rotation, translation);
}

void readCopy()
{
  int index;
  for (index=0; index<input.readBufSize; ++index)
  {
    if (input.readBuf[index] == '\n')
    {
      input.packetBuf[input.packetBufPosition] = '\0';/*terminate string*/

      if (input.packetBuf[0] == 'd' && input.packetBufPosition == 15)
	processDisplacementPacket();
      else if (input.packetBuf[0] == 'k' && input.packetBufPosition == 4)
	processButtonKPacket();
      else if (input.packetBuf[0] == 'c' && input.packetBufPosition == 3)
	processButtonCPacket();
      else if (input.packetBuf[0] == 'n' && input.packetBufPosition == 2)
	processButtonNPacket();
      else if (input.packetBuf[0] == 'q' && input.packetBufPosition == 3)
	processButtonQPacket();
      else
	printf("unknown packet   %s\n",input.packetBuf);
      input.packetBufPosition=0;
    }
    else
    {
      input.packetBuf[input.packetBufPosition] = input.readBuf[index];
      input.packetBufPosition++;
      if (input.packetBufPosition == MAXPACKETSIZE)
      {
	input.packetBufPosition = 0;
	printf("packet buffer overrun\n");
      }
    }
  }
}
