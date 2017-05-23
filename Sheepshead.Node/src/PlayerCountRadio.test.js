import React from 'react';
import { shallow } from 'enzyme';
import PlayerCountRadio from './PlayerCountRadio';

test('PlayerCountRadio title is Dwayne', () => {
    const pcRadio = shallow(
        <PlayerCountRadio name="fred" title="dwayne"/>
    );

    expect(pcRadio.find('span.title').text()).toEqual('dwayne');
});

test('PlayerCountRadio groups have name fred', () => {
    const pcRadio = shallow(
        <PlayerCountRadio name="fred" title="dwayne" />
    );

    pcRadio.find('input[type="radio"]').forEach(function (node) {
        expect(node.prop('name')).toEqual('fred');
    });
});

test('PlayerCountRadio should have new value of 2', () => {
    const pcRadio = shallow(
        <PlayerCountRadio name="sam" title="bill" />
    );

    pcRadio.find('input[type="radio"]').at(2).simulate('click');
    expect(pcRadio.update().find('.value').props().value).toEqual(2);
});