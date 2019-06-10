import React, { Component } from 'react';
import { Collapse } from 'react-bootstrap';

export default class NexPanel extends Component {
    static defaultProps = {
        defaultOpened: true
    }

    constructor(props) {
        super(props);

        this.state = {
            open: props.defaultOpened
        };
    }

    toggle = () => this.setState(prevState => ({ open: !prevState.open }));

    render() {
        return (
            <div className="nex-panel">
                <div className="panel-header" onClick={this.toggle}>
                    {this.props.title}
                </div>
                <Collapse in={this.state.open}>
                    <div>
                        {this.props.children}
                    </div>
                </Collapse>
            </div>
        );
    }
}