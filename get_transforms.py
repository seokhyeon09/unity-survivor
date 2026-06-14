import re

with open('Assets/PlayerMonsterCustomWork/Scenes/Player+monster.unity', 'r', encoding='utf-8') as f:
    content = f.read()

objects = content.split('--- !u!')

transforms = {}
gameobjects = {}

for obj in objects:
    if 'GameObject:' in obj:
        match = re.search(r'^1 &(\d+)', obj)
        if match:
            id = match.group(1)
            name_match = re.search(r'm_Name:\s*(.+)', obj)
            if name_match:
                name = name_match.group(1).strip()
                transform_match = re.search(r'- component: \{fileID: (\d+)\}', obj)
                if transform_match:
                    gameobjects[transform_match.group(1)] = name
    elif 'Transform:' in obj:
        match = re.search(r'^4 &(\d+)', obj)
        if match:
            id = match.group(1)
            pos_match = re.search(r'm_LocalPosition: \{x: ([^,]+), y: ([^,]+), z: ([^\}]+)\}', obj)
            if pos_match:
                transforms[id] = {'pos': f"X:{pos_match.group(1)}, Y:{pos_match.group(2)}, Z:{pos_match.group(3)}"}

for t_id, data in transforms.items():
    if t_id in gameobjects:
        name = gameobjects[t_id]
        if 'Weapon' in name or 'FirePoint' in name or 'Pistol' in name or 'Shotgun' in name or 'MachineGun' in name:
            print(f'{name} -> Pos: {data["pos"]}')
