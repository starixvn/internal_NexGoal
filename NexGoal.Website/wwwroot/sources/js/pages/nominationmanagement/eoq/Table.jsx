import React, { Component, Fragment } from 'react';

import Row from './Row';
import CreateEOQ from './Create';

function THead(props) {
    return (
        <thead>{props.children}</thead>
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

        this.state = {
            showModal: false
        };
    }

    closeModal() {
        this.setState({ showModal: false });
    }

    showModal() {
        this.setState({ showModal: true });
    }

    render() {
        return (
            <Fragment>
                <table className="table table-bordered">
                    <THead>
                        {this.props.isHost &&
                            <tr className="header-control">
                                <th colSpan="9">
                                    <a className="pull-left btn btn-primary" onClick={this.showModal.bind(this)}>
                                        <i className="fa fa-plus"></i>Create Nomination Period
                                </a>
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
                    </THead>
                    <TBody>
                        {this.props.eoqs.length === 0 &&
                            <tr>
                                <td className="empty-result" colSpan="9">No information available!</td>
                            </tr>
                        }
                        {this.props.eoqs.length > 0 &&
                            this.props.eoqs.map((eoq, index) => {
                                return (
                                    <Row key={'eoq-' + eoq.eoqyId}
                                        rowIndex={index + 1}
                                        eoq={eoq}
                                        isHost={this.props.isHost} />)
                            })
                        }
                    </TBody>
                </table>
                <div>
                    {this.state.showModal &&
                        <CreateEOQ showModal={this.state.showModal} closeModal={this.closeModal.bind(this)} />
                    }
                </div>
            </Fragment>
        );
    }
}

export default Table;