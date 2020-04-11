# -*- coding: utf-8 -*-
"""
Created on Sun Mar  3 15:02:21 2019

@author: S77
"""
import random


class Color:
    """ Color class, that contains three RGB color components and magic number of
    color, that uses Tekla. RGB in DEC (from 0 to 255)

    """

    def __init__(self, r, g, b):
        self.r = r
        self.g = g
        self.b = b
        self.neh = self.calcneh(r, g, b)

    def calcneh(self, r, g, b):
        """ Find magic number of color for Tekla """
        hexsum = 0
        for comp in [r, g, b]:
            hexsum += hex(comp).split('x')[-1]
        return int(hexsum, 16) + 100


def make_PObjGrp_files(profiles):
    example_file = open("attributes/Profile.PObjGrp.Example", "r")
    example_data = example_file.read()
    example_file.close()

    for prof_name in profiles:
        new_file = open("attributes/" + "@" + prof_name + ".PObjGrp", "w+")
        new_data = example_data.replace("$$$$$", prof_name)
        new_file.write(new_data)
        new_file.close()


def create_colors_list(colors, profiles):
    for prof_num in range(len(profiles)):
        colors[prof_num] = Color(
            random.randint(0, 255),
            random.randint(0, 255),
            random.randint(0, 255))
    return None


def make_rep_file(profiles, colors):
    rep_file = open("attributes/Profile_Colors.rep", "w+")
    rep_file.write('REPRESENTATIONS \n { \n    Version= 1.04 \n')
    rep_file.write('    Count= ' + str(len(profiles)) + ' \n')

    example_file = open("attributes/Profile.rep.Example")
    example_par = example_file.read()
    example_file.close()

    for j in range(len(profiles)):
        new_par = example_par.replace('$PROF', '@' + profiles[j])
        new_par = new_par.replace('$NEH', str(colors[j].neh))
        new_par = new_par.replace('$RED', str(colors[j].r))
        new_par = new_par.replace('$GREEN', str(colors[j].g))
        new_par = new_par.replace('$BLUE', str(colors[j].b))
        rep_file.write(new_par)

    rep_file.close()
    return None


def main():
    profiles = []
    # Считываем список профилей
    try:
        with open("Отчеты/all_prof.xsr") as all_prof:
            profiles = all_prof.readlines()
    except FileNotFoundError:
        print("Нужно создать отчет all_prof.xsr и положить его в папку 'Отчеты'")

    # Форматируем список профилей
    for i in range(len(profiles)):
        try:
            if profiles[i].isspace():
                profiles.pop(i)
            profiles[i] = profiles[i].strip()
        except IndexError:
            continue

    colors = [None] * len(profiles)
    create_colors_list(colors, profiles)

    make_PObjGrp_files(profiles)
    make_rep_file(profiles, colors)


if __name__ == '__main__':
    main()
