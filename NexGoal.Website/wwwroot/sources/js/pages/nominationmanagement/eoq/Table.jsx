import React, { Component } from 'react';

function Header(props) {
    return (
        <thead>
            {props.isHost &&
                <tr className="header-control">
                    <th colSpan="9">
                        <a className="pull-left btn btn-primary"><i className="fa fa-plus"></i>Create Nomination Period</a>
                    </th>
                </tr>
            }
            <tr>
                <th>#</th>
                <th>Quarter</th>
                <th>Year</th>
                <th>Start Date For Nominator</th>
                <th>End Date For Nominator</th>
                <th>Remind Date For Nominator</th>
                <th>End Date For Committee</th>
                <th>Status</th>
                <th>Action</th>
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
            <table className="table table-bordered">
                <Header isHost={this.props.isHost} />
                <TBody>
                    {this.props.eoqs.length === 0 &&
                        <tr>
                            <td className="empty-result" colSpan="9">No information available!</td>
                        </tr>
                    }
                </TBody>
            </table>
        );
    }
}

export default Table;