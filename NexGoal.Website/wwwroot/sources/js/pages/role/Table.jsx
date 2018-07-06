import React, { Component } from 'react';

import Row from './Row';

function Header() {
    return (
        <thead>
            <tr>
                <th rowSpan="2">Function</th>
                <th colSpan="2">Role Permission</th>
            </tr>
            <tr>
                <th>Read</th>
                <th>Host</th>
            </tr>
        </thead>
    );
}

function TBody(props) {
    return (
        <tbody>{props.children}</tbody>
    );
}

class Table extends Component {
    constructor(props) {
        super(props);
    }

    render() {
        return (
            <table className="table table-bordered assign-permission">
                <Header />
                <TBody>
                    {
                        this.props.functions.map(func => {
                            return (
                                <Row key={'function-' + func.functionId}
                                    function={func}
                                    isHost={this.props.isHost}
                                    handleCheckboxChange={this.props.handleCheckboxChange} />)
                        })
                    }
                </TBody>
            </table>
        );
    }
}

export default Table;