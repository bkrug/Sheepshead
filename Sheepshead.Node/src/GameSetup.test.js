import React from 'react';
import { shallow } from 'enzyme';
import GameSetup from './GameSetup';

test('GameSetup initialy counts 0 players.', () => {
    const gameSetup = shallow(
        <GameSetup />
    );

    expect(gameSetup.find('span.totalPlayers').text()).toEqual("0");
});

test('GameSetup counts 3 players when we only select 3 humans.', () => {
    const gameSetup = shallow(
        <GameSetup />
    );

    gameSetup.find('PlayerCountRadio[name="humans"]').simulate('change', 'humans', 3);
    gameSetup.update();
    expect(gameSetup.find('span.totalPlayers').text()).toEqual("3");
});

test('GameSetup counts 4 players when we click 3 newbie and 1 learning.', () => {
    const gameSetup = shallow(
        <GameSetup />
    );

    gameSetup.find('PlayerCountRadio[name="newbie"]').simulate('change', 'newbie', 3);
    gameSetup.find('PlayerCountRadio[name="learning"]').simulate('change', 'learning', 1);
    gameSetup.update();
    expect(gameSetup.find('span.totalPlayers').text()).toEqual("4");
});

test('GameSetup counts 5 players when we click 3 newbie and 1 learning and then change to 4 newbie.', () => {
    const gameSetup = shallow(
        <GameSetup />
    );

    gameSetup.find('PlayerCountRadio[name="newbie"]').simulate('change', 'newbie', 3);
    gameSetup.find('PlayerCountRadio[name="learning"]').simulate('change', 'learning', 1);
    gameSetup.find('PlayerCountRadio[name="newbie"]').simulate('change', 'newbie', 4);
    gameSetup.update();
    expect(gameSetup.find('span.totalPlayers').text()).toEqual("5");
});