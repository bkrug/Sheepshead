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

test('PlayerCountRadio click event should trigger a change event', () => {
    var actGroupName, actPlayerCount;
    var changeFunction = function (groupName, playerCount) { actGroupName = groupName; actPlayerCount = playerCount; };
    const pcRadio = shallow(
        <PlayerCountRadio name="dwight" title="dwayne" onChange={changeFunction}/>
    );

    pcRadio.find('input[type="radio"]').at(3).simulate('click');
    expect(actGroupName).toEqual("dwight");
    expect(actPlayerCount).toEqual(3);
});

test('PlayerCountRadio remaining property of 3 should cause 2 buttons to be disabled.', () => {
    const pcRadio = shallow(
        <PlayerCountRadio name="dwight" title="dwayne" remaining="3"/>
    );

    var radios = pcRadio.find('input[type="radio"]');
    expect(radios.at(0).prop('disabled')).toEqual(undefined);
    expect(radios.at(1).prop('disabled')).toEqual(undefined);
    expect(radios.at(2).prop('disabled')).toEqual(undefined);
    expect(radios.at(3).prop('disabled')).toEqual(undefined);
    expect(radios.at(4).prop('disabled')).toEqual('disabled');
    expect(radios.at(5).prop('disabled')).toEqual('disabled');
});

test('PlayerCountRadio remaining property of 2 should cause 1 buttons to be disabled.', () => {
    const pcRadio = shallow(
        <PlayerCountRadio name="dwight" title="dwayne" value="2" remaining="2" />
    );

    var radios = pcRadio.find('input[type="radio"]');
    expect(radios.at(0).prop('disabled')).toEqual(undefined);
    expect(radios.at(1).prop('disabled')).toEqual(undefined);
    expect(radios.at(2).prop('disabled')).toEqual(undefined);
    expect(radios.at(3).prop('disabled')).toEqual(undefined);
    expect(radios.at(4).prop('disabled')).toEqual(undefined);
    expect(radios.at(5).prop('disabled')).toEqual('disabled');
});