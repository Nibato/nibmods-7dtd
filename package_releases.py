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

def getModInfo(path: str):
    info_path = os.path.join(path,'ModInfo.xml')

    if not os.path.isfile(info_path) or os.path.isfile(os.path.join(path, 'skip')):
        return None

    try:
        root = ET.parse(info_path)
    except ET.ParseError as e:
        print("Error parsing '{}': {}".format(path, e))
        return None

    authorElement = root.find('.//ModInfo/Author[@value]')
    versionElement = root.find('.//ModInfo/Version[@value]')

    if authorElement is None or versionElement is None:
        return None
    
    name = os.path.basename(path)
    author = authorElement.attrib['value']
    version = versionElement.attrib['value']

    if author.casefold() != EXPECTED_AUTHOR.casefold():
        return None

    return ModInfo(name, path, author, version)


def packageMod(mod: ModInfo):
    archive_name = "{}_v{}".format(mod.name, mod.version)
    archive_path = os.path.join(RELEASE_DIR, archive_name)

    base_path = os.path.basename(mod.path)

    print("Creating package '{}'".format(archive_name))

    shutil.make_archive(archive_path, 
        format='zip',
        root_dir=MODS_PATH,
        base_dir=base_path
        )
    pass

def main():
    mods = []

    if not os.path.isdir(MODS_PATH):
        print("Could not access mods directory")

    for d in os.listdir(MODS_PATH):
        path = os.path.join(MODS_PATH,d)

        modInfo = getModInfo(path)

        if modInfo is None:
            continue

        mods.append(modInfo)

    if len(mods) <= 0:
        print("No eligible mods found")
        return
    
    if os.path.isdir(RELEASE_DIR):
        shutil.rmtree(RELEASE_DIR)

    os.makedirs(RELEASE_DIR)

    for mod in mods:
        packageMod(mod)
    

if __name__ == "__main__":
    main()