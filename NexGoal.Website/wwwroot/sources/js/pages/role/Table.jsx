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

class Table extends Component {
    constructor(props) {
        super(props);
    }

    render() {
        return (
            <table className="table table-bordered assign-permission">
                <Header />
                <tbody>
                    {
                        this.props.functions.map(func => {
                            return (
                                <Row key={func.functionId}
                                    function={func}
                                    isHost={this.props.isHost}
                                    handleCheckboxChange={this.props.handleCheckboxChange} />)
                        })
                    }
                </tbody>
            </table>
        );
    }
}

export default Table;