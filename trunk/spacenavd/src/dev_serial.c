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

#include "stdio.h"
#include "string.h"
#include "dev_serial.h"
#include "serial/sball.h"
#include "serial/smag.h"

#define VERSION_STRING_MAX 64
#define NAME_MAX 64

struct Device{
  char name[NAME_MAX];
  char version_string[VERSION_STRING_MAX];
  int (*open_func)(const char *devfile);
  int (*close_func)();
  int (*read_func)(struct dev_input *inp);
  int (*get_fd_func)();
} device;

int open_dev_serial(const char *devfile)
{
	clear_device();
	
	/*defaulting to libsball functions*/
	device.open_func = sball_open;
	device.close_func = sball_close;
	device.read_func = sball_get_input;
	device.get_fd_func = sball_get_fd;

	if (detect_device(devfile) == -1 || device.open_func == 0){
		clear_device();
		return -1;
	}
	if(device.open_func(devfile)==-1) {
		clear_device();
		return -1;
	}
	if (device.get_fd_func)
	  return device.get_fd_func();
	else
	  return -1;
}

void close_dev_serial()
{
	if (device.close_func != 0)
		device.close_func();
	clear_device();
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
	device.open_func = 0;
	device.close_func = 0;
	device.read_func = 0;
	device.get_fd_func = 0;
}

int detect_device(const char *devfile)
{
	if (open_file(devfile) < 0)
		  return -1;
	setup_port();
	get_version_string(device.version_string, VERSION_STRING_MAX);
	close_smag();
	if (strlen(device.version_string)>0)
		  printf("%s\n", device.version_string);	
	setup_device();  
	return 0;  
}

void setup_device()
{
	char *instance;
	derive_device_name();
	instance = strstr(device.version_string, "MAGELLAN");
	if (instance){
	  device.open_func = open_smag;
	  device.close_func = close_smag;
	  device.read_func = read_smag;
	  device.get_fd_func = get_fd_smag;
	}
}

void derive_device_name()
{
	char *instance;
	instance = strstr(device.version_string, "MAGELLAN  Version 6.60");
	if (instance){
		strcpy(device.name, "Magellan Plus XT");
		return;
	}
	
	instance = strstr(device.version_string, "MAGELLAN  Version 5.79");
	if (instance){
		strcpy(device.name, "Magellan Classic");
		return;
	}
}