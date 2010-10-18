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

#ifndef DEV_SERIAL_H_
#define DEV_SERIAL_H_

#include "event.h"
#include "serial/serialconstants.h"


enum {UNKNOWN, BALL_1003, BALL_2003B, BALL_2003C, BALL_3003C, BALL_4000FLX, BALL_5000FLX, MOUSE_CLASSIC, MOUSE_PLUS_XT};

struct Device{
  char name[NAME_MAX];
  char version_string[VERSION_STRING_MAX];
  int device_model;
  int (*open_func)(const char *devfile);
  int (*close_func)();
  int (*read_func)(struct dev_input *inp);
  int (*get_fd_func)();
};

int open_dev_serial(const char *devfile);
void close_dev_serial(void);
int read_dev_serial(struct dev_input *inp);
void clear_device();
void derive_device_name();
void setup_device();

#endif	/* DEV_SERIAL_H_ */
