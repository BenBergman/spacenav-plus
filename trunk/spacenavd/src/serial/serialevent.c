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

#include <stdlib.h>
#include "serialevent.h"

int evpool_size=0;
static struct event *ev_free_list;

struct event *alloc_event(void)
{
  struct event *ev;
  
  if(ev_free_list) {
    ev = ev_free_list;
    ev_free_list = ev->next;
  } else {
    ev = malloc(sizeof *ev);
    evpool_size++;
  }
  return ev;
}

void free_event(struct event *ev)
{
  if(evpool_size > 512) {
    free(ev);
    evpool_size--;
  } else {
    ev->next = ev_free_list;
    ev_free_list = ev;
  }
}
