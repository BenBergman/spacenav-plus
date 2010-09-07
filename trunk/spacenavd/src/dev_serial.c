/*
spacenavd - a free software replacement driver for 6dof space-mice.
Copyright (C) 2007-2010 John Tsiombikas <nuclear@member.fsf.org>

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

#include "dev_serial.h"
#include "serial/sball.h"

struct Device{
  char name[64];
  char version_string[128];
  int (*open_func)(const char *devfile);
  int (*close_func)();
  int (*read_func)(struct dev_input *inp);  
} device;

int open_dev_serial(const char *devfile)
{
	int signal;
	clear_device();
	
	device.open_func = sball_open;
	device.close_func = sball_close;
	device.read_func = sball_get_input;
	
	
	if (device.open_func == 0)
		return -1;	
	signal = device.open_func(devfile);
	if(signal==-1) {
		return -1;
	}
	
	return sball_get_fd();
}

void close_dev_serial()
{
	if (device.close_func != 0)
		device.close_func();
}

int read_dev_serial(struct dev_input *inp)
{
	if (device.read_func == 0)
		return -1;
	if(!device.read_func(inp)) {
		return -1;
	}
	return 0;
}

void clear_device()
{
	device.name[0]='\0';
	device.version_string[0]='\0';
	device.open_func=0;
	device.close_func=0;
	device.read_func=0;
}
