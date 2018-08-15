import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import PlayerCountRadio from './PlayerCountRadio';
import OnOffRadio from './OnOffRadio';

export interface GameSetupState {
    value: number;
    partnerCard: boolean;
    remaining: number;
    gameName: string;
    selections: { [index: string]: number };
    leastersDefault: boolean;
}

export class GameSetup extends React.Component<RouteComponentProps<{}>, GameSetupState> {
    readonly MAX_PLAYERS = 5;
    readonly HUMANS = "humanCount";
    readonly NEWBIE = "newbieCount";
    readonly BASIC = "basicCount";

    constructor(props: any) {
        super(props);
        console.log(props);
        let selections: { [index: string]: number } = {};
        selections[this.HUMANS] = selections[this.NEWBIE] = selections[this.BASIC] = 0;
        this.state = {
            value: 0,
            partnerCard: true,
            remaining: this.MAX_PLAYERS,
            gameName: '',
            selections: selections,
            leastersDefault: this.getLeastersDefault(props)
        };
        this.handleChange = this.handleChange.bind(this);
    }

    private getLeastersDefault(props: any): boolean {
        var queryParams = props.location.search
            .replace('?', '')
            .split('&')
            .map(function (q: string) { return q.split('='); });
        var leastersParam = queryParams.filter(function (kvp: string) { return kvp[0] == 'leastersOn' });
        console.log(leastersParam);
        return leastersParam.length == 0 ? true : leastersParam[0][1].toLowerCase() == 'true';
    }

    handleChange(radioGroup: string, radioValue: number) {
        if (radioValue === 0 || radioValue > 0) {
            this.state.selections[radioGroup] = radioValue;
            let newTotal = this.state.selections[this.HUMANS]
                + this.state.selections[this.NEWBIE]
                + this.state.selections[this.BASIC];
            this.setState({ value: newTotal, remaining: this.MAX_PLAYERS - newTotal });
        }
    }

    playerCountValidityStyle() {
        return this.validPlayerCount() ? '' : 'invalid';
    }

    validPlayerCount() {
        return (this.state.value === 3 || this.state.value === 5);
    }

    public render() {
        return (
            <div className="gameSetup">
                <h4>Setup Sheepshead Game</h4>
                <form method="post">
                    <PlayerCountRadio name={this.HUMANS} title="Humans" onChange={this.handleChange} value={this.state.selections[this.HUMANS]} remaining={this.state.remaining} />
                    <PlayerCountRadio name={this.NEWBIE} title="A.I. Simple" onChange={this.handleChange} value={this.state.selections[this.NEWBIE]} remaining={this.state.remaining} />
                    <PlayerCountRadio name={this.BASIC} title="A.I. Basic" onChange={this.handleChange} value={this.state.selections[this.BASIC]} remaining={this.state.remaining} />
                    <div>
                        <span>Total Players:</span>
                        <span className={"totalPlayers " + this.playerCountValidityStyle()}>{this.state.value}</span>
                    </div>
                    <input type="hidden" className="remaining" value={this.state.remaining} />
                    <OnOffRadio name="partnerCard" title="Partner Card" onText="Jack of Hearts" offText="Called Ace" defaultValue={true} disabled={this.state.value != 5} />
                    <OnOffRadio name="leastersGame" title="Leasters On" onText="on" offText="off" defaultValue={this.state.leastersDefault} disabled={false} />
                    <input type="submit" value="Play" disabled={!this.validPlayerCount()} />
                </form>
            </div>
        );
    }
}