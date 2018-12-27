import '../../css/setup.css';
import * as React from 'react';
import { RouteComponentProps } from 'react-router';

export interface PlayerCountState {
    title: string;
    value: number;
    onChange: any;
    name: string;
}

//export default class PlayerCountRadio extends React.Component<RouteComponentProps<{}>, PlayerCountState> {
export default class PlayerCountRadio extends React.Component<any, any> {
    constructor(props: PlayerCountState) {
        super(props);
        console.log(props.title);
        this.state = {
            value: props.value || 0,
            title: props.title,
            onChange: props.onChange,
            name: props.name
        };
        this.handleClick = this.handleClick.bind(this);
    }

    handleClick(i: number) {
        if (this.state.onChange)
            this.state.onChange(this.state.name, i);
    }

    getValue() {
        return this.props.value;
    }

    renderRadio(i: number) {
        return (
            <span>
                <input
                    type="radio"
                    name={this.state.name}
                    value={i}
                    onClick={() => this.handleClick(i)}
                    onChange={() => function () { return; }}
                    checked={this.props.value === i}
                    disabled={this.props.remaining + this.props.value < i || this.state.name == 'humanCount' && i == 0}
                />{i}
            </span>
        );
    }

    public render() {
        return (
            <div className="playerCountRadio" onChange={this.state.onChange}>
                <div className="title-container"><span>{this.props.title}</span></div>
                <div className="button-container">
                    {this.renderRadio(0)}
                    {this.renderRadio(1)}
                    {this.renderRadio(2)}
                    {this.renderRadio(3)}
                    {this.renderRadio(4)}
                    {this.renderRadio(5)}
                </div>
                <input type="hidden" className="value" value={this.props.value} />
            </div>
        );
    }
}