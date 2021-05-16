const UPDATE_INTERVAL = 1000;

const state = {
    favorites: [],
}

const processItem = (parent, item) => {
    if (!item) {
        return;
    }

    if (item.nodeType !== undefined) {
        parent.appendChild(item);
    }
    else if (typeof item != "object") {
        const elem = document.createTextNode(item);
        parent.appendChild(elem);
    }
    else {
        for (const kid of item) {
            processItem(parent, kid);
        }
    }
}

const e = (name, attrs, kids) => {
    const elem = document.createElement(name);
    for (const a in attrs) {
        if (a == 'style' || a == 'dataset') {
            for (const s in attrs[a]) {
                elem[a][s] = attrs[a][s];
            }
        }
        else {
            elem[a] = attrs[a];
        }
    }

    processItem(elem, kids);

    return elem;
}

const equip = async (item, side) => {
    side = side || 'right'
    const res = await fetch('/api/command', {
        method: 'POST',
        body: JSON.stringify({ command: `player.equipitem ${item.formId.toString(16).padStart(8, '0')} 0 ${side}` }),
        headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/json'
        },
    });
    const status = await res.json()
};

const reloadFavorites = async () => {
    try {
        const res = await fetch('/api/favorites', { headers: { 'Accept': 'application/json' } });
        const favorites = await res.json()
        let different = false;
        for (const f in favorites) {
            const favorite = favorites[f];
            const orig = state.favorites[f];
            if (!orig || orig.formId != favorite.formId) {
                different = true;
                break;
            }
        }

        if (!different) {
            return;
        }

        state.favorites = favorites;
        render();
    }
    catch (e) {
        console.error(e);
    }

    setTimeout(reloadFavorites, UPDATE_INTERVAL)
};

const render = async () => {
    try {
        const content = e('div', { id: 'content' }, [
            e('div', { className: 'favorites favorites__left' }, [
                e('h1', null, 'Left Hand'),
                e('ul', null, state.favorites.map(item =>
                    e('li', null,
                        e('button', { onclick: () => equip(item, 'left') }, item.itemName),
                    ),
                )),
            ]),
            e('div', { className: 'favorites favorites__right' }, [
                e('h1', null, 'Right Hand'),
                e('ul', null, state.favorites.map(item =>
                    e('li', null,
                        e('button', { onclick: () => equip(item, 'right') }, item.itemName),
                    ),
                )),
            ]),
        ]);
        document.getElementById("content").replaceWith(content);
    }
    catch (e) {
        console.error(e);
    }
};

setTimeout(reloadFavorites, UPDATE_INTERVAL)
