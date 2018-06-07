import * as React from 'react';

const style: React.CSSProperties = {
    height: '12rem',
    width: '12rem',
    marginRight: '1.5rem',
    marginBottom: '1.5rem',
    color: 'white',
    padding: '1rem',
    textAlign: 'center',
    fontSize: '1rem',
    lineHeight: 'normal',
    float: 'left',
}

export default class CardDropBox extends React.Component<any, any> {
    public render() {
        return (
            <div style={{ ...style }} ></div>
        )
    }
}