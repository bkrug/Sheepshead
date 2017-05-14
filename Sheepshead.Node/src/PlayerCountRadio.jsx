import React from 'react';

export default class PlayerCountRadio extends React.Component {
    render() {
        return (
            <div className="playerCountRadio">
                <span>{this.props.title}</span>
                <input type="radio" name={this.props.name} value="0" />0
                <input type="radio" name={this.props.name} value="1" />1
                <input type="radio" name={this.props.name} value="2" />2
                <input type="radio" name={this.props.name} value="3" />3
                <input type="radio" name={this.props.name} value="4" />4
                <input type="radio" name={this.props.name} value="5" />5
            </div>
        );
    }
}