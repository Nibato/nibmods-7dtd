#!/usr/bin/env python3

import xml.etree.ElementTree as ET
import os
from dataclasses import dataclass
import shutil


MODS_PATH = r'C:\Program Files (x86)\Steam\steamapps\common\7 Days To Die\Mods'
EXPECTED_AUTHOR = 'nibato'

RELEASE_DIR = os.path.join(os.getcwd(), "PackagedReleases")


@dataclass
class ModInfo:
    name: str
    path: str
    author: str
    version: str


def get_mod_info(path: str):
    info_path = os.path.join(path,'ModInfo.xml')

    if not os.path.isfile(info_path) or os.path.isfile(os.path.join(path, 'skip')):
        return None

    try:
        root = ET.parse(info_path)
    except ET.ParseError as e:
        print("Error parsing '{}': {}".format(path, e))
        return None

    author_element = root.find('.//ModInfo/Author[@value]')
    version_element = root.find('.//ModInfo/Version[@value]')

    if author_element is None or version_element is None:
        return None

    name = os.path.basename(path)
    author = author_element.attrib['value']
    version = version_element.attrib['value']

    if author.casefold() != EXPECTED_AUTHOR.casefold():
        return None

    return ModInfo(name, path, author, version)


def package_mod(mod: ModInfo):
    archive_name = "{}_v{}".format(mod.name, mod.version)
    archive_path = os.path.join(RELEASE_DIR, archive_name)

    base_path = os.path.join(os.path.basename(MODS_PATH), os.path.basename(mod.path))

    print("Creating package '{}'".format(archive_name))

    shutil.make_archive(archive_path,
        format='zip',
        root_dir=os.path.dirname(MODS_PATH),
        base_dir=base_path
        )


def main():
    mods = []

    if not os.path.isdir(MODS_PATH):
        print("Could not access mods directory")

    for d in os.listdir(MODS_PATH):
        path = os.path.join(MODS_PATH,d)

        mod_info = get_mod_info(path)

        if mod_info is None:
            continue

        mods.append(mod_info)

    if len(mods) <= 0:
        print("No eligible mods found")
        return

    if os.path.isdir(RELEASE_DIR):
        shutil.rmtree(RELEASE_DIR)

    os.makedirs(RELEASE_DIR)

    for mod in mods:
        package_mod(mod)


if __name__ == "__main__":
    main()
