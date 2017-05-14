import React from 'react';
import { shallow } from 'enzyme';
import PlayerCountRadio from './PlayerCountRadio';

test('PlayerCountRadio title is Dwayne', () => {
    const checkbox = shallow(
        <PlayerCountRadio name="fred" title="dwayne"/>
    );

    expect(checkbox.find('span').text()).toEqual('dwayne');
});

test('PlayerCountRadio groups have name fred', () => {
    const checkbox = shallow(
        <PlayerCountRadio name="fred" title="dwayne" />
    );

    checkbox.find('input').forEach(function (node) {
        expect(node.prop('name')).toEqual('fred');
    });
});