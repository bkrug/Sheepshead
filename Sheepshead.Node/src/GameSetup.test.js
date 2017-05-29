import React from 'react';
import { shallow } from 'enzyme';
import GameSetup from './GameSetup';

test('This test does nothing', () => {
    const pcRadio = shallow(
        <GameSetup />
    );

    expect(1).toEqual(1);
});