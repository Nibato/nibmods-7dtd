#!/usr/bin/env python3

import xml.etree.ElementTree as ET
import os
from dataclasses import dataclass
import shutil
from zipfile import ZipFile, ZIP_DEFLATED


MODS_PATH = r'C:\Program Files (x86)\Steam\steamapps\common\7 Days To Die\Mods'
EXPECTED_AUTHOR = 'nibato'

WORKING_DIR = os.path.dirname(__file__)
RELEASE_DIR = os.path.join(WORKING_DIR, 'PackagedReleases')
LICENSE_PATH = os.path.join(WORKING_DIR, 'LICENSE.md')
GAME_VERSION = 'A20'


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
        print('Error parsing \'{}\': {}'.format(path, e))
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
    archive_name = '{}_v{}_({}).zip'.format(mod.name, mod.version, GAME_VERSION)
    archive_path = os.path.join(RELEASE_DIR, archive_name)

    base_path = os.path.join('Mods', os.path.basename(mod.path))

    print('Creating package \'{}\''.format(archive_name))

    with ZipFile(archive_path, 'w', compression=ZIP_DEFLATED, compresslevel=9) as zp:
        #add mod files
        for root, _, files in os.walk(mod.path):
            for f in files:
                path = os.path.join(root, f)
                rel_path = os.path.join(base_path, os.path.relpath(path, mod.path))

                zp.write(path, arcname=rel_path)

        # add license
        license_arcname = ''.join([os.path.splitext(os.path.basename(LICENSE_PATH))[0], '.txt'])
        with open(LICENSE_PATH, 'r') as fp:
            zp.writestr(license_arcname, fp.read())
            
        # Generate readme
        readme_includes = [
                os.path.join(WORKING_DIR, os.path.basename(mod.path), 'README.md'), 
                os.path.join(WORKING_DIR, 'INSTALL.md')
            ]

        readme_text = ['# {} v{}'.format(mod.name, mod.version)]

        # add readme if available
        for path in readme_includes:
            if not os.path.isfile(path):
                continue

            with open(path, 'r') as fp:
                readme_text.append(fp.read())

        if len(readme_text) > 1:
            zp.writestr('README.txt', '\n\n'.join(readme_text))


def main():
    mods = []

    if not os.path.isdir(MODS_PATH):
        print('Could not access mods directory')

    for d in os.listdir(MODS_PATH):
        path = os.path.join(MODS_PATH,d)

        mod_info = get_mod_info(path)

        if mod_info is None:
            continue

        mods.append(mod_info)

    if len(mods) <= 0:
        print('No eligible mods found')
        return

    if os.path.isdir(RELEASE_DIR):
        shutil.rmtree(RELEASE_DIR)

    os.makedirs(RELEASE_DIR)

    for mod in mods:
        package_mod(mod)


if __name__ == '__main__':
    main()
