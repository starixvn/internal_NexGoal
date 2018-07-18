import React, { Component, Fragment } from 'react';

import Row from './Row';
import CreateEOQ from './Create';
import DeleteEOQ from './Delete';

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
            showCreateModal: false,
            showDeleteModal: false,
            eoqSelected: null
        };
    }

    showCreateModal() {
        this.setState({ showCreateModal: true });
    }

    closeCreateModal() {
        this.setState({ showCreateModal: false });
    }

    showDeleteModal(eoqSelected) {
        this.setState({
            showDeleteModal: true,
            eoqSelected: eoqSelected
        });
    }

    closeDeleteModal() {
        this.setState({
            showDeleteModal: false,
            eoqSelected: null
        });
    }

    render() {
        const isHost = this.props.isHost;
        const eoqs = this.props.eoqs;

        return (
            <Fragment>
                <table className="table table-bordered">
                    <THead>
                        {isHost &&
                            <tr className="header-control">
                                <th colSpan="9">
                                    <a className="pull-left btn btn-primary" onClick={this.showCreateModal.bind(this)}>
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
                        {eoqs.length === 0 &&
                            <tr>
                                <td className="empty-result" colSpan="9">No information available!</td>
                            </tr>
                        }
                        {eoqs.length > 0 &&
                            eoqs.map((eoq, index) => {
                                return (
                                    <Row key={'eoq-' + eoq.eoqyId}
                                        rowIndex={index + 1}
                                        eoq={eoq}
                                        isHost={isHost}
                                        showDeleteModal={this.showDeleteModal.bind(this)} />)
                            })
                        }
                    </TBody>
                </table>

                {
                    this.state.showCreateModal &&
                    <CreateEOQ
                        showModal={this.state.showCreateModal}
                        closeModal={this.closeCreateModal.bind(this)}
                        handleFilterSubmit={this.props.handleFilterSubmit} />
                }
                {
                    this.state.showDeleteModal &&
                    <DeleteEOQ
                        eoqSelected={this.state.eoqSelected}
                        showModal={this.state.showDeleteModal}
                        closeModal={this.closeDeleteModal.bind(this)}
                        handleFilterSubmit={this.props.handleFilterSubmit} />
                }
            </Fragment>
        );
    }
}

export default Table;